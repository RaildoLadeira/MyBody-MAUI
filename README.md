# 🏋️ MyBody — Fitness & Nutrition Management App

**MyBody** is a full-featured cross-platform application built with **C#** and **.NET MAUI**, designed to help users track workout routines, manage daily nutrition, and monitor body progression metrics.

---

## 🚀 Key Features

- **⚙️ Goals & Hydration:** Automatically calculates daily water intake goals based on body weight ($35\text{ ml/kg}$).
- **🍎 Nutritional Logger:** Calorie and macro calculator supporting both gram-based and unit-based foods (e.g., eggs vs. chicken breast).
- **🏋️ Smart Workout Routines:**
  - Day-by-day split training routines (Workouts A through E).
  - Exercise selection grouped by muscle target with custom input fallback.
  - Interactive exercise checklist with visual status updates (strikethrough / completed highlight) and a daily reset option.
  - Automatic persistent storage for added, modified, or deleted exercises.
- **📈 Progress & Body Metrics:**
  - Automatic Body Mass Index (BMI) calculation with weight status classification.
  - Basal Metabolic Rate (BMR) and Total Daily Energy Expenditure (TDEE) estimation using the Harris-Benedict formula.
  - Weight tracking history.
  - Local image gallery for body transformation photos.
- **🌐 Real-Time Internationalization (i18n):** Dynamic runtime switching between **English**, **Portuguese**, and **Spanish** across all tabs, menus, and controls.
- **🌙 Dark Mode & Light Mode Support:** Built-in dynamic theme toggling adapted for desktop (Windows) and mobile environments.
- **💾 Local Data Persistence:** Secure offline data storage using `Preferences` and JSON serialization (`System.Text.Json`).

---

## 🛠️ Tech Stack

- **Language:** C# (.NET 8 / .NET 9)
- **Framework:** .NET MAUI (Multi-platform App UI)
- **UI & Layout:** XAML, AppThemeBinding, Responsive Grid & Flex Layouts
- **Architecture & Persistence:** System.Text.Json, Local Preferences Storage, Event-Driven UI Architecture

---

## 👨‍💻 Developer

**Raildo Santos Ladeira**  
*C# & .NET Developer*
