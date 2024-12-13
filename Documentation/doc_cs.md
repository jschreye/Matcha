# Guide : Création et Gestion des Tables, Services et Repositories

Ce document explique les étapes à suivre pour ajouter une nouvelle table dans le projet et intégrer les services, DTO, et repositories associés, tout en respectant les bonnes pratiques.

---

## 1. **Création d’une nouvelle table**
Lorsque vous créez une nouvelle table dans la base de données :

1. **Créer une entité (Entity)** dans le projet **Core** :
   - L’entité représente votre table au niveau du code.
   - Elle doit contenir toutes les colonnes de la table sous forme de propriétés.

2. **Créer un DTO (Data Transfer Object)** dans le projet **Core** :
   - Le DTO sert à transporter et manipuler les données dans le code sans toucher directement à l'entité ou à la table.
   - Cela permet de limiter les dépendances entre votre application et votre base de données.

### **Pourquoi utiliser un DTO ?**
Un **DTO (Data Transfer Object)** est une classe simple qui contient des propriétés permettant de transporter des données entre différentes couches de l’application. Voici ses principaux avantages :

- **Découplage** : Les DTO permettent de séparer la structure des données utilisées dans la couche métier de celle utilisée dans la base de données.
- **Protection des données** : En manipulant les DTO au lieu des entités, vous évitez de modifier directement la structure de la table.
- **Optimisation** : Vous pouvez inclure uniquement les champs nécessaires à une opération ou une vue.
- **Facilité de modification** : En cas de changement dans la table ou l’entité, seules les transformations entre entité et DTO doivent être ajustées.

#### **Exemple de DTO**
Si vous avez une entité `User` contenant plusieurs champs :
```csharp
public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Password { get; set; }
}
```

Vous pouvez créer un DTO qui expose uniquement les champs pertinents pour une opération :
```csharp
public class UserDto
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
}
```

---

## 2. **Création d’un Service**

1. **Créer un service** dans le projet **Infrastructure** :
   - Ce service contiendra les méthodes métier pour gérer les données liées à la table.

2. **Créer une interface pour ce service** dans le projet **Core** :
   - L’interface définit les signatures des méthodes du service, sans implémentation.
   - Par exemple :

```csharp
public interface IUserService
{
    Task<List<UserDto>> GetAllUsersAsync();
    Task AddUserAsync(UserDto user);
    Task DeleteUserAsync(int id);
}
```

3. **Faire le lien entre le service et son interface** :
   - Le service doit implémenter l'interface. Exemple :

```csharp
public class UserService : IUserService
{
    // Implémentation des méthodes de l'interface
}
```

4. **Enregistrer le service dans `Program.cs`** :
   - Ajoutez le service au conteneur d’injection de dépendances :

```csharp
builder.Services.AddScoped<IUserService, UserService>();
```

---

## 3. **Création d’un Repository**

1. **Créer un repository** dans le projet **Infrastructure** :
   - Le repository est responsable des interactions avec la base de données.

2. **Créer une interface pour le repository** dans le projet **Core** :
   - L’interface définit les signatures des méthodes du repository.

3. **Faire le lien entre le repository et son interface** :
   - Exemple :

```csharp
public class UserRepository : IUserRepository
{
    // Implémentation des méthodes pour interagir avec la base de données
}
```

4. **Enregistrer le repository dans `Program.cs`** :
   - Ajoutez le repository au conteneur d’injection de dépendances :

```csharp
builder.Services.AddScoped<IUserRepository, UserRepository>();
```

---

## 4. **Utilisation des Services et Repositories**

### 4.1 Injecter un repository dans un service
Pour utiliser un repository dans un service, injectez son interface dans le constructeur du service :

```csharp
public class UserService : IUserService
{
    private readonly IUserRepository _repository;

    public UserService(IUserRepository repository)
    {
        _repository = repository;
    }
}
```

### 4.2 Appeler un service depuis un autre service
Le processus est similaire à l’injection d’un repository. Injectez simplement l’interface du service nécessaire dans le constructeur :

```csharp
public class AnotherService : IAnotherService
{
    private readonly IUserService _userService;

    public AnotherService(IUserService userService)
    {
        _userService = userService;
    }
}
```

---

## 5. **Exemple global**
Voici un exemple simplifié pour une table `User` :

### 5.1. Entité
```csharp
public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

### 5.2. DTO
```csharp
public class UserDto
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
}
```

### 5.3. Interface du service
```csharp
public interface IUserService
{
    Task<List<UserDto>> GetAllUsersAsync();
    Task AddUserAsync(UserDto user);
    Task DeleteUserAsync(int id);
}
```

### 5.4. Service
```csharp
public class UserService : IUserService
{
    private readonly IUserRepository _repository;

    public UserService(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        return await _repository.GetAllUsersAsync();
    }

    public async Task AddUserAsync(UserDto user)
    {
        await _repository.AddUserAsync(user);
    }

    public async Task DeleteUserAsync(int id)
    {
        await _repository.DeleteUserAsync(id);
    }
}
```

### 5.5. Interface du repository
```csharp
public interface IUserRepository
{
    Task<List<UserDto>> GetAllUsersAsync();
    Task AddUserAsync(UserDto user);
    Task DeleteUserAsync(int id);
}
```

### 5.6. Repository
```csharp
public class UserRepository : IUserRepository
{
    private readonly string? _connectionString;

    public UserRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        // Implémentation de l'accès à la base de données
    }

    public async Task AddUserAsync(UserDto user)
    {
        // Implémentation de l'insertion d'un utilisateur
    }

    public async Task DeleteUserAsync(int id)
    {
        // Implémentation de la suppression d'un utilisateur
    }
}
```

---



