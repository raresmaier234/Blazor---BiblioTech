# 📚 BiblioTech - Modern Library Management System

O aplicație Blazor Server modernă și elegantă pentru gestionarea unei biblioteci personale de cărți, cu funcționalități avansate de analytics, export și notificări.

![Blazor](https://img.shields.io/badge/Blazor-Server-512BD4?logo=blazor)
![.NET](https://img.shields.io/badge/.NET-7.0-512BD4?logo=.net)
![Entity Framework](https://img.shields.io/badge/Entity_Framework-Core-512BD4)
![SQLite](https://img.shields.io/badge/SQLite-Database-003B57?logo=sqlite)

## 🌟 Funcționalități Principale

### 📊 Dashboard cu Analytics
- **4 Statistici în Timp Real**: Total cărți, autori, categorii, și adăugări lunare
- **3 Tipuri de Grafice Interactive**:
  - Bar Chart pentru distribuția cărților pe ani
  - Horizontal Bar Chart pentru top 5 categorii
  - Leaderboard cu medalii pentru top 5 autori
- **Info Panel**: Metrici calculate dinamic (media cărți/autor, cea mai recentă carte, interval ani)
- **Quick Actions**: Link-uri rapide către secțiuni importante

### 📥 Export CSV
- Export complet al tuturor cărților
- Format CSV standard (RFC 4180)
- Include: titlu, autor, email, an, categorii, dată adăugare
- Download automat fără refresh

### 🔔 Toast Notifications
- **4 Tipuri**: Success, Error, Info, Warning
- **Design Modern**: Animații smooth cu bounce effect, gradients, iconițe
- **Auto-dismiss**: Dispare după 3 secunde
- **Responsive**: Adaptat perfect pe mobile
- **Feedback Instant** pentru toate operațiunile CRUD

### 📖 Gestionare Complete
- **Cărți**: CRUD complet cu relații many-to-many
- **Autori**: Gestionare cu validare email
- **Categorii**: Organizare flexibilă

### 🔍 Filtrare Avansată
- Căutare după titlu
- Filtrare după autor
- Filtrare după an publicare
- Filtrare după categorie
- Query string persistence

### ✨ Validare Completă
- Client-side cu Data Annotations
- Server-side în business logic
- Visual feedback (border roșu, mesaje eroare)
- Toast notifications pentru status

## 🏗️ Arhitectură

### Clean Architecture - Separation of Concerns
```
┌─────────────────────────────────┐
│   Razor Pages (UI Layer)        │
│   - Books.razor                  │
│   - Authors.razor                │
│   - Categories.razor             │
│   - Dashboard.razor              │
└─────────────┬───────────────────┘
              │
┌─────────────▼───────────────────┐
│   Page Managers (Logic Layer)   │
│   - BookPageManager              │
│   - AuthorPageManager            │
│   - CategoryPageManager          │
│   - DashboardManager             │
└─────────────┬───────────────────┘
              │
┌─────────────▼───────────────────┐
│   Services (Data Access Layer)  │
│   - BookService                  │
│   - AuthorService                │
│   - CategoryService              │
└─────────────┬───────────────────┘
              │
┌─────────────▼───────────────────┐
│   Entity Framework Core          │
│   - LibraryContext               │
│   - SQLite Database              │
└──────────────────────────────────┘
```

### Beneficii
- ✅ **Testabilitate**: Managers pot fi unit tested
- ✅ **Reusabilitate**: Logică separată de UI
- ✅ **Maintainability**: Cod organizat
- ✅ **Scalability**: Ușor de extins

## 🚀 Tehnologii

- **Blazor Server** (.NET 7) - Framework UI reactive
- **Entity Framework Core** - ORM pentru database
- **SQLite** - Database embedded
- **Bootstrap 5** - Styling de bază
- **Custom CSS** - Design modern cu gradients și animații
- **JavaScript Interop** - Pentru download și toast notifications

## 📱 Design Responsive

- **Desktop**: Layout complet cu sidebar fix
- **Tablet**: Grid adaptat pentru ecrane medii
- **Mobile**: Stacked layout cu butoane full-width

## 🎨 Design Modern

### Visual Identity
- **Gradiente Vibrante**: Purple, Blue, Green pentru identitate vizuală
- **Animații Smooth**: Transitions și transforms pe hover
- **Iconițe Emoji**: Pentru o interfață prietenoasă
- **Shadow Effects**: Depth și hierarchy vizual
- **Border Radius**: 16px pentru look modern

### UX Features
- **Feedback Instant**: Toast notifications pentru toate acțiunile
- **Loading States**: Mesaje de așteptare clare
- **Empty States**: Mesaje friendly când nu există date
- **Error Handling**: Mesaje descriptive de eroare
- **Hover Effects**: Micro-interacțiuni pe toate elementele clickable

## 📊 Statistici Proiect

- **Total Lines of Code**: ~3,800
- **Entități**: 4 (Book, Author, Category, BookCategory)
- **Razor Pages**: 6
- **Managers**: 5
- **Services**: 3
- **Migrations**: 4
- **CSS Classes**: 100+
- **JavaScript Functions**: 2

## 🚀 Cum să Rulezi

```bash
# Clone repository
git clone https://github.com/raresmaier234/Blazor---BiblioTech.git

# Navigate to project
cd BlazorLibraryApp

# Restore packages
dotnet restore

# Run migrations (dacă e nevoie)
dotnet ef database update

# Run application
dotnet run

# Deschide browser la
https://localhost:5249
```

## 📖 Documentație Detaliată

Pentru detalii complete despre funcționalități și implementare, vezi [FEATURES.md](FEATURES.md)

## 🎯 Punți Cheie pentru Interviu

1. **Clean Architecture**: Separation of concerns cu 3 layere distincte
2. **Modern UX**: Toast notifications și animații profesionale
3. **Business Value**: Export CSV pentru rapoarte
4. **Analytics**: Dashboard cu statistici și grafice în timp real
5. **Best Practices**: DI, patterns, validare, error handling
6. **Responsive**: Design adaptat pentru toate device-urile

## 🔮 Viitor - Posibile Extensii

- 🌙 Dark Mode toggle
- ⭐ Star Ratings pentru cărți
- ❤️ Favorites marking
- 🔍 Global search cross-entity
- 💾 Backup/Restore database
- 📄 PDF Reports export
- 📧 Email notifications
- 📈 Reading progress tracking

## 👨‍💻 Autor

**Rares Maier**
- GitHub: [@raresmaier234](https://github.com/raresmaier234)

## 📄 Licență

Acest proiect este creat în scop educațional și pentru demonstrare în interviuri tehnice.

---

⭐ **Dacă îți place proiectul, lasă un star pe GitHub!**
