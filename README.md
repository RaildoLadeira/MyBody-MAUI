# 📱 MyBody - Cross-Platform Fitness & Nutrition Manager

![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![.NET MAUI](https://img.shields.io/badge/.NET%20MAUI-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![XAML](https://img.shields.io/badge/XAML-0078D4?style=for-the-badge&logo=xaml&logoColor=white)
![Platform](https://img.shields.io/badge/Platform-Android%20%7C%20iOS%20%7C%20Windows-lightgrey?style=for-the-badge)

**MyBody** é um aplicativo mobile multiplataforma desenvolvido em **.NET MAUI** e **C#** focado no acompanhamento diário de treinos, planejamento nutricional e acompanhamento de metas corporais.

---

## 🌍 Recursos Principais (Features)

- **Internationalization (i18n):** Suporte nativo e alternância em tempo real para 3 idiomas (**Português, Inglês e Espanhol**) através do `LocalizationService`.
- **Gestão de Treinos Fichas Personalizadas:** Seleção dinâmica de exercícios por grupo muscular, formatação automática de carga (kg) e repetições, com checklist diário.
- **Protocolos de Nutrição & Dieta:** Acompanhamento de refeições adaptado à rotina do usuário (*Treino Noturno*, *Treino Vespertino* e *Dia Sem Treino*) com cálculo automático de calorias.
- **Acompanhamento Corporal & Metas:** Registro de peso, altura, % de gordura (BF) e cálculo de macros de acordo com o objetivo (*Cutting, Bulking, Maintenance*).
- **Suporte UX/UI:** Compatibilidade nativa com Dark Mode (Modo Escuro) e interfaces responsivas.

---

## 🛠️ Tecnologias e Arquitetura

- **Linguagem & Framework:** C# / .NET MAUI 10
- **Interface:** XAML com Data Binding e arquitetura orientada a objetos (POO)
- **Persistência de Dados:** Estrutura para banco de dados local (`DatabaseService.cs`)
- **Padrão de Tradução:** Dicionário estático responsivo a eventos de troca de cultura (`OnLanguageChanged`)

---

## 📱 Estrutura do Projeto

```text
MyBody/
├── Models/              # Modelos de dados (RotinaModel, Exercicio, Refeicao)
├── Services/            # Serviços (LocalizationService, DatabaseService, PlanoService)
├── Views/              # Páginas da aplicação (MainPage, TreinosPage, EvolucaoPage)
└── AppShell.xaml        # Estrutura de navegação por abas (TabBar)
