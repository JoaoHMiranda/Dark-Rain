# 🌧️ DARK RAIN: Relatório Técnico Master de Desenvolvimento

**Engine:** Unity 6 (LTS) | **Pipeline:** Universal Render Pipeline (URP)

---

## 1. Introdução

**Dark Rain** é uma aplicação interativa 3D de tempo real que combina mecânicas de ação isométrica (*Top-Down Shooter*) com sistemas de progressão procedural (*Roguelike*). O projeto foi estruturado para demonstrar a aplicação prática e integrada dos pilares teóricos da Computação Gráfica: **Modelagem Geométrica, Texturização PBR, Iluminação Volumétrica e Matemática Vetorial**.


Este documento descreve a arquitetura de software, a organização de dados e as soluções técnicas implementadas para atender aos requisitos de avaliação.

---

## 🏗️ PARTE 1: ESTRUTURA DO PROJETO (File System)

A organização do projeto segue padrões de arquitetura de software para *Real-Time Engines*, separando estritamente Lógica (Scripts), Dados (ScriptableObjects) e Recursos Visuais (Assets).

### Hierarquia de Pastas (`/Assets`)

*   **📂 Scenes:** Gerenciamento de estados de aplicação.
    *   `MainMenu.unity`: Cena de entrada, responsável pela inicialização de sistemas e configuração de preferências.
    *   `Game.unity`: Cena principal onde ocorre o ciclo de simulação (*Game Loop*).
*   **📂 Scripts:** Código fonte C# modularizado.
    *   *Subpastas:* Organizadas por contexto (ex: `/Skills` para lógica de dados, `/Managers` para singletons).
*   **📂 ScriptableObjects:** Banco de dados relacional e serializável.
    *   `/Skills`: Arquivos individuais (`.asset`) contendo atributos de habilidades (Vida, Dano, Velocidade).
    *   `/Trees`: Definições das estruturas de dados das árvores de evolução.
    *   `/Fusions`: Definições lógicas para combinação de habilidades.
*   **📂 Prefabs:** Objetos pré-configurados (Instâncias).
    *   `Player`: O objeto complexo do jogador (Hierarquia Física + Visual).
    *   `Ghost`: O inimigo configurado com materiais, IA e colissores.
    *   `Arrow_Final`: O projétil com componentes físicos (`Rigidbody`).
    *   `SkillCard_Prefab`: Elemento de UI instanciável para o menu dinâmico.
*   **📂 Models:** Arquivos FBX brutos e metadados de importação.
    *   `/Erika Archer`: Malha poligonal do player e clipes de animação.
    *   `/ghost`: Malha estática do inimigo.
*   **📂 Materials:** Definições de superfície PBR, controlando mapas de Albedo, Normal e Metallic.

---

## 🎨 PARTE 2: DETALHAMENTO DA MODELAGEM GEOMÉTRICA

Esta seção detalha os ativos 3D, justificando sua escolha e configuração técnica conforme os requisitos de **Modelagem e Hierarquia** da disciplina.

### 1. Personagem Principal (Erika Archer)
*   **Tipo:** Malha Poligonal com *Rigging* (Esqueleto Hierárquico).
*   **Origem:** Mixamo (Adobe).
*   **Geometria:** Otimizada para renderização em tempo real (*Real-Time Rendering*).
*   **Hierarquia Interna:** O modelo possui uma estrutura de nós (ossos) parentais (*Hips -> Spine -> Shoulder -> Arm*) manipulada pelo sistema **Mecanim**.
*   **Configuração de Importação:**
    *   *Rig:* Humanoid (Permite o *retargeting* de animações genéricas).
    *   *Avatar Definition:* Create From This Model (Gera o mapa de ossos).
    *   *Materials:* Extraídos para permitir a injeção de mapas PBR.

### 2. Inimigo (Ghost)
*   **Tipo:** Malha Estática animada via translação de *Transform*.
*   **Escala e Unidades:** O modelo original foi reescalado na importação (*Scale Factor*) para corresponder ao sistema métrico da Unity (1 unidade = 1 metro), garantindo coerência espacial com o cenário.
*   **Topologia de Colisão:** A malha visual é encapsulada por um `BoxCollider` ou `CapsuleCollider` invisível, simplificando a geometria complexa do fantasma para cálculos físicos eficientes na CPU.

### 3. Projéteis e Cenário
*   **Flecha:** Modelo 3D simples. O pivô foi ajustado para o centro de massa e a rotação alinhada ao eixo Z (*Forward*) para garantir que a física de translação mova a flecha "de ponta".
*   **Chão (Ground):** Um plano primitivo (`Quad`) com mapeamento UV ajustado via técnica de *Tiling* (repetição) para renderizar a textura de grama em alta resolução sem distorção.

---

## 💻 PARTE 3: CATÁLOGO DE SCRIPTS (Lógica do Sistema)

Descrição técnica da responsabilidade de cada classe C# dentro da arquitetura.

### 🧠 Core (Jogador)
*   **`PlayerMovement.cs`**: Processa a entrada do usuário (Input System) e executa a translação do vetor de posição no espaço global (`Space.World`). Aplica *Deadzones* matemáticas para filtrar ruído de input.
*   **`PlayerAttack.cs`**: Executa a matemática de combate. Realiza *Raycasting* da câmera para o plano do chão, calcula Quatérnios de rotação para o eixo Y, instancia projéteis (Factory Pattern) e controla a velocidade da animação via parâmetros.
*   **`PlayerHealth.cs`**: Gerencia a Máquina de Estados de vida. Ao morrer, altera o `Rigidbody` para *Kinematic* (travando a física newtoniana), desliga colisores e aplica vetores de força (`AddForce`) radiais nos inimigos.
*   **`PlayerExperience.cs`**: Gerencia a curva logarítmica de XP e a coleta magnética de orbes via detecção de proximidade (`OverlapSphere`).

### 👻 IA e Combate
*   **`EnemyAI.cs`**: Algoritmo de perseguição vetorial (`Target - Self`). Normaliza o vetor resultante para mover o inimigo em direção ao jogador a velocidade constante.
*   **`EnemyHealth.cs`**: Gerencia o estado de dano. Executa a destruição do objeto e instanciação de partículas e loot.
*   **`ArrowBehavior.cs`**: Controlador balístico. Move o objeto no vetor *Forward*, gerencia o ciclo de vida (Garbage Collection preventivo) e detecta colisão, aplicando dano e momento linear (*Knockback*).

### 🎲 Sistema Roguelike (Dados & UI)
*   **`SkillManager.cs`**: Gerenciador de dados. Implementa o algoritmo **Fisher-Yates Shuffle** para garantir aleatoriedade justa no sorteio de cartas e valida matematicamente a condição de "Max Level".
*   **`LevelUpManager.cs`**: Controlador de UI. Gerencia uma pilha (Stack) de níveis pendentes e manipula o `Time.timeScale` para congelar o delta-tempo da física durante a interface.
*   **`HUDManager.cs`**: Controlador de visualização. Utiliza corrotinas e a função matemática `Mathf.Lerp` para interpolar valores de interface suavemente.

---

## 🔍 PARTE 4: ANÁLISE TÉCNICA DETALHADA (27 Pontos)


### 1. Visão Geral e Arquitetura

**1. Definição do Gênero e Escopo Técnico**
Dark Rain é um *Top-Down Shooter* focado na interação geometria-luz-física. O projeto foi arquitetado para demonstrar renderização e simulação em tempo real.

**2. Pipeline de Renderização (URP)**
Utilização do *Universal Render Pipeline* para iluminação otimizada via *Single-Pass Forward Rendering*, essencial para manter a performance com múltiplas luzes dinâmicas.

**3. Arquitetura de Cena (Scene Graph)**
A cena é organizada como um Grafo de Cena hierárquico complexo. O uso de parentesco (Parent/Child) propaga transformações geométricas de forma controlada.

**4. Iluminação Volumétrica e Atmosfera**
Atendendo ao requisito de volume: A `Directional Light` atua como luz de preenchimento noturna (azulada). O volume é gerado por uma `SpotLight` no jogador com *Soft Shadows* ativadas, forçando o cálculo de mapas de sombra em tempo real.

**5. Gerenciamento de Estado (Game Loop)**
Controle determinístico de estados (Menu, Gameplay, Pause, Over) via manipulação de `Time.timeScale`. A interface usa `unscaledDeltaTime` para animar enquanto o mundo físico está congelado.

### 2. Personagens e Modelagem

**6. Modelagem Hierárquica do Jogador**
Separação entre "Raiz Física" (Collider) e "Filho Visual" (Mesh). Isso permite aplicar matrizes de rotação no modelo visual para mirar sem rotacionar a caixa de colisão física quadrada, evitando bugs de geometria.

**7. Instanciamento (Prefabs)**
Uso de *Prefabs* (Instâncias) para renderizar múltiplos inimigos com baixo custo de memória, aproveitando o *Batching* dinâmico da GPU (Requisito do Slide 3).

**8. Detalhamento da Malha**
O modelo principal possui uma topologia otimizada para deformação esquelética. A importação configurou o Avatar Mecanim para permitir o reuso de animações.

**9. Materiais PBR e Mapas de Textura**
Uso do fluxo PBR (Slide 4). Inimigos usam *Normal Maps* para simular relevo sem polígonos extras e baixo *Smoothness* para simular tecido fosco. O Player usa alto *Metallic* para reflexos especulares na armadura.

**10. Animação por Máquina de Estados (FSM)**
Uso do Mecanim para gerenciar transições (Idle → Run → Attack) via parâmetros e gatilhos lógicos.

**11. Layers e Avatar Masks**
Segmentação do esqueleto via *Avatar Mask*. A "Base Layer" controla as pernas (locomoção) e a "UpperBody Layer" controla o torso (ataque), permitindo *Blending* simultâneo de animações.

**12. Sincronia Procedural**
A velocidade da animação é ajustada via código (`anim.SetFloat`) inversamente proporcional ao `FireRate`, demonstrando controle matemático sobre o tempo de animação.

### 3. Mecânicas de Gameplay e Física

**13. Movimentação Vetorial**
O input é tratado como vetor normalizado e traduzido para coordenadas globais, com aplicação de "Zona Morta" para precisão.

**14. Raycasting (Matemática de Interseção)**
Conversão de coordenadas 2D (Tela) para 3D (Mundo) através da projeção de um raio contra um Plano Matemático, calculando o ponto exato de mira.

**15. Colisão 2.5D (Topologia)**
Para corrigir a perspectiva isométrica, os colisores dos projéteis foram estendidos verticalmente (Eixo Y), garantindo interseção mesmo com inimigos de malha baixa.

**16. Física Newtoniana (Knockback)**
Aplicação da Segunda Lei de Newton (`F = m.a`). Ao colidir, o projétil aplica um vetor de força impulsiva (`AddForce`) ao corpo rígido do inimigo.

**17. Data-Driven Design**
Uso de `ScriptableObjects` para separar dados (Balanceamento) da lógica (Código), tornando o sistema modular e escalável.

**18. Aleatoriedade Justa**
Implementação do algoritmo **Fisher-Yates Shuffle** para garantir distribuição uniforme e justa das cartas de habilidade, evitando repetições viciadas.

### 4. Interface e Fluxo

**19. UI Responsiva**
Uso de âncoras (*Anchors*) e *Layout Groups* no Canvas para garantir que a interface se adapte matematicamente a qualquer resolução de tela.

**20. Interpolação Linear (Lerp)**
As barras de HUD não "pulam" de valor. Elas utilizam a função `Mathf.Lerp` para transitar suavemente entre valores, aplicando conceitos de animação procedural 2D (Requisito do Slide 6).

**21. Feedback de Morte**
Sequência programada de eventos: trava de física, troca de animação e delay de UI (`Invoke`), garantindo clareza semântica no estado de derrota.

**22. Paleta de Cores e Semântica**
Design cromático funcional: Vermelho (Vida/Urgência), Azul (XP/Progresso), Dourado (Sucesso/Max Level) e Fundo Escuro para contraste e legibilidade.

**23. Feedback Visual e Transições**
Uso de *Alpha Blending* (transparência) nos painéis para manter o contexto do jogo visível durante a pausa, melhorando a UX.

**24. Otimização de Textura (Tiling)**
Aplicação de mapeamento UV com repetição (*Tiling*) e texturas *Seamless* no cenário para evitar estiramento visual (*Stretching*) e manter a resolução.

**25. Câmera Isométrica**
Configuração de projeção perspectiva com ângulo fixo e script de interpolação (`LateUpdate`) para seguimento suave do alvo sem vibração (*Jitter*).

**26. Implementação de Áudio**
Uso de ouvintes espaciais e fontes de áudio (`AudioSource`) atreladas a eventos de animação e física para feedback sensorial completo.

**27. Processo de Build**
Compilação final para executável Windows (`.exe`), encapsulando configurações de resolução, input e qualidade gráfica, entregando um produto de software autônomo.
