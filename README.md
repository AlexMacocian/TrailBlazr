# TrailBlazr

[![CI](https://github.com/your-username/TrailBlazr/actions/workflows/ci.yaml/badge.svg)](https://github.com/your-username/TrailBlazr/actions/workflows/ci.yaml)
[![NuGet](https://img.shields.io/nuget/v/TrailBlazr.svg)](https://www.nuget.org/packages/TrailBlazr/)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://opensource.org/licenses/MIT)

TrailBlazr is a powerful library for Blazor that enables compile-correct view management with linked ViewModels through the MVVM pattern. It provides a seamless navigation system using a view container component and view manager, making it easy to build maintainable Blazor applications with strong typing and separation of concerns.

## Features

- 🔗 **Strongly-typed View-ViewModel binding** - Compile-time safety for view-viewmodel relationships
- 🧭 **Navigation management** - Centralized view navigation through `IViewManager`
- 🏗️ **MVVM pattern support** - Clean separation of concerns with ViewModels
- 📦 **Dependency injection integration** - Seamless integration with .NET DI container
- 🎯 **Dynamic view rendering** - Efficient view switching with `ViewContainer` component
- ⚡ **Minimal setup** - Easy configuration with extension methods

## Installation

Install TrailBlazr via NuGet Package Manager:

```bash
dotnet add package TrailBlazr
```

Or via Package Manager Console:

```powershell
Install-Package TrailBlazr
```

## Quick Start

### 1. Register TrailBlazr services

In your `Program.cs` or startup configuration:

```csharp
using TrailBlazr.Extensions;

// Add TrailBlazr services
builder.Services.AddTrailBlazr();

// Register your views and viewmodels
builder.Services.RegisterView<HomeView, HomeViewModel>();
builder.Services.RegisterView<UserProfileView, UserProfileViewModel>();

var app = builder.Build();
```

### 2. Create a ViewModel

```csharp
using TrailBlazr.ViewModels;

public class HomeViewModel : ViewModelBase<HomeViewModel, HomeView>
{
    public string Title { get; set; } = "Welcome to TrailBlazr!";
    public string Message { get; set; } = "Navigate with confidence.";
}
```

### 3. Create a View

```razor
@using TrailBlazr.Views
@inherits ViewBase<HomeView, HomeViewModel>

<div class="home-container">
    <h1>@ViewModel.Title</h1>
    <p>@ViewModel.Message</p>
    
    <button @onclick="() => NavigateToProfile()">
        Go to Profile
    </button>
</div>

@code {
    [Inject]
    private IViewManager ViewManager { get; set; }

    private void NavigateToProfile()
    {
        this.ViewManager.ShowView<UserProfileView, UserProfileViewModel>();
    }
}
```

### 4. Add ViewContainer to your layout

```razor
@using TrailBlazr.Components

<div class="main-content">
    <ViewContainer />
</div>
```

### 5. Navigate between views

Inject `IViewManager` anywhere in your application to navigate:

```csharp
[Inject]
private IViewManager ViewManager { get; set; }

// Navigate to a view with optional data context
ViewManager.ShowView<UserProfileView, UserProfileViewModel>(userData);

// Or navigate by type
ViewManager.ShowView(typeof(HomeView), someData);
```

## Core Concepts

### ViewBase<TView, TViewModel>

Base class for all views that provides:
- Strong typing between view and viewmodel
- Automatic viewmodel injection
- Access to `ViewManager` for navigation

### ViewModelBase<TViewModel, TView>

Base class for all viewmodels that provides:
- Strong typing between viewmodel and view
- Foundation for implementing business logic
- Inherits from `INotifyPropertyChanged` for data binding

### IViewManager

Central service for managing view navigation:
- `ShowView<TView, TViewModel>(object? dataContext)` - Navigate with strong typing
- `ShowView(Type viewType, object? dataContext)` - Navigate by type
- `ShowViewRequested` event - Subscribe to navigation events

### ViewContainer

Blazor component that hosts and renders the current view:
- Automatically updates when navigation occurs
- Supports dynamic component rendering
- Passes data context to views

## Architecture

TrailBlazr follows the MVVM (Model-View-ViewModel) pattern:

```
┌─────────────────┐    ┌──────────────────┐    ┌─────────────────┐
│      View       │◄──►│    ViewModel     │◄──►│     Model       │
│   (Razor)       │    │   (C# Class)     │    │   (Data)        │
└─────────────────┘    └──────────────────┘    └─────────────────┘
         ▲                        ▲
         │                        │
         ▼                        ▼
┌─────────────────┐    ┌──────────────────┐
│  ViewContainer  │◄──►│   ViewManager    │
│  (Component)    │    │   (Service)      │
└─────────────────┘    └──────────────────┘
```

## Requirements

- .NET 9.0 or later
- Blazor Hybrid or Blazor Server/WebAssembly