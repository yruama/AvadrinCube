# Guide de Migration et Setup - Cube Project Refactoring

## Résumé des Améliorations Réalisées

Ce document résume tous les changements effectués pour améliorer la qualité du code.

### ✅ Bugs Critiques Corrigés

1. **BinaryFormatter Remplacé** 
   - ❌ Ancien: `SaveManager.cs` utilisait `BinaryFormatter` (API dépréciée)
   - ✅ Nouveau: Utilise `JsonSerializationUtility` (compatible .NET 8+)
   - **Fichier**: [SaveManager.cs](Assets/Scripts/Manager/SaveManager.cs)

2. **Quaternion Order Bug Fixé**
   - ❌ Ancien: `CustomVector4.GetQuaternion()` retournait `Quaternion(w, x, y, z)`
   - ✅ Nouveau: Retourne `Quaternion(x, y, z, w)` (correct)
   - **Fichier**: [SaveManager.cs](Assets/Scripts/Manager/SaveManager.cs#L105)

3. **Lerp Bug Corrigé**
   - ❌ Ancien: `Mathf.Lerp(minDistance, minDistance, value)` (même valeur deux fois)
   - ✅ Nouveau: Code logique correct pour distance-based volume
   - **Fichier**: [SoundManager.cs](Assets/Scripts/Manager/SoundManager.cs)

4. **Condition Impossible Fixée**
   - ❌ Ancien: `if (positionsEnregistrees.Count < 0)` (impossible)
   - ✅ Nouveau: `if (positionsEnregistrees.Count == 0)`
   - **Fichier**: [PlayerTutorial.cs](Assets/Scripts/Player/PlayerTutorial.cs#L30)

5. **Performance Debug Logs Supprimés**
   - ❌ Ancien: `Debug.Log()` dans Update() → garbage chaque frame
   - ✅ Nouveau: Logs supprimés
   - **Fichier**: [PlayerJump.cs](Assets/Scripts/Player/PlayerJump.cs)

### ✅ Élimination des Magic Strings (GameObject.Find)

**Nouvelle Architecture**: `GameRegistry` (Singleton Pattern)

#### Avant:
```csharp
_diamondManager = GameObject.Find("GameManager").GetComponent<DiamondManager>();
_player = GameObject.Find("Player");
await Utils.Functions.ShowCanvasGroup(GameObject.Find("BlackScreen").GetComponent<CanvasGroup>());
```

#### Après:
```csharp
_diamondManager = GameRegistry.Instance.GetDiamondManager();
_player = GameRegistry.Instance.GetPlayerTransform().gameObject;
await Utils.Functions.ShowCanvasGroup(GameRegistry.Instance.GetBlackScreen());
```

**Fichiers Modifiés**:
- [GameRegistry.cs](Assets/Scripts/Manager/GameRegistry.cs) - Nouveau
- [DiamondController.cs](Assets/Scripts/Props/DiamondController.cs)
- [PlayerController.cs](Assets/Scripts/Player/PlayerController.cs)
- [SpeedRunManager.cs](Assets/Scripts/Manager/SpeedRunManager.cs)
- [TrailRenderer.cs](Assets/Scripts/Effects/TrailRenderer.cs)
- [PlayerTeleport.cs](Assets/Scripts/Player/PlayerTeleport.cs)

### ✅ Constantes Centralisées (GameConstants)

**Nouveau Fichier**: [GameConstants.cs](Assets/Scripts/Utils/GameConstants.cs)

Remplace tous les hardcoded strings:
```csharp
// Tags
TAG_PLAYER = "Player"
TAG_ENEMY = "ennemy"
TAG_PROJECTILE = "Projectile"
TAG_PROJECTILE_PARRY = "ProjectileParry"
TAG_WALL = "Wall"

// Object Names
OBJECT_TO_ACTIVATE = "OBJECT_TO_ACTIVATE"

// Physics
GRAVITY_MULTIPLIER = 9.81f
```

**Fichiers Modifiés** (13 occurrences):
- [PlayerController.cs](Assets/Scripts/Player/PlayerController.cs)
- [PlayerDetector.cs](Assets/Scripts/Player/PlayerDetector.cs)
- [DiamondController.cs](Assets/Scripts/Props/DiamondController.cs)
- [Projectile.cs](Assets/Scripts/Obstacle/Projectile.cs)
- [Teleport.cs](Assets/Scripts/Props/Teleport.cs)
- [Switches.cs](Assets/Scripts/Props/Switches.cs)
- [SoundManager.cs](Assets/Scripts/Manager/SoundManager.cs)
- [PlayerJump.cs](Assets/Scripts/Player/PlayerJump.cs)

### ✅ Sérialisation JSON (Nouvelle Utility)

**Nouveau Fichier**: [JsonSerializationUtility.cs](Assets/Scripts/Utils/JsonSerializationUtility.cs)

Remplace l'approche BinaryFormatter par JSON moderne:
```csharp
JsonSerializationUtility.SaveToJson<DataPlayer>(path, data);
DataPlayer data = JsonSerializationUtility.LoadFromJson<DataPlayer>(path);
```

### ✅ Amélioration Null Safety

Ajout de null checks systématiques:

```csharp
// SoundManager - Check Player tag
GameObject playerObject = GameObject.FindGameObjectWithTag(GameConstants.TAG_PLAYER);
if (playerObject == null) {
    Debug.LogError("Player not found");
    enabled = false;
    return;
}

// DiamondController - Check GameRegistry
_diamondManager = GameRegistry.Instance.GetDiamondManager();
if (_diamondManager == null) {
    Debug.LogError("DiamondManager not found");
    enabled = false;
    return;
}
```

---

## Setup Instructions

### 1. GameRegistry Setup (IMPORTANT)

Pour que le projet fonctionne, vous DEVEZ:

1. Créer un GameObject vide appelé "GameRegistry" dans votre scène principale
2. Ajouter le composant `GameRegistry` à cet objet
3. Assigner les références suivantes dans l'Inspector:

```
GameRegistry Inspector Setup:
├─ Player References
│  ├─ Player Transform: [drag votre joueur]
│  └─ Player Controller: [drag le component PlayerController]
├─ Game Managers
│  ├─ Game Manager: [drag GameManager]
│  ├─ Save Manager: [drag SaveManager]
│  ├─ Diamond Manager: [drag DiamondManager]
│  ├─ Speed Run Manager: [drag SpeedRunManager]
│  └─ Sound Manager: [drag SoundManager]
├─ UI Elements
│  ├─ Black Screen: [drag le CanvasGroup]
│  └─ Fade Canvas: [drag le CanvasGroup]
└─ Game Objects
   └─ Object To Activate: [drag l'objet]
```

### 2. Validation des Tags

Vérifiez que vos tags Unity existent:
- "Player" ✓
- "ennemy" ✓
- "Projectile" ✓
- "ProjectileParry" ✓
- "Wall" ✓

### 3. Testing

1. Entrez en Play Mode
2. Vérifiez que GameRegistry valide toutes les références
3. Les warnings apparaîtront dans la Console si quelque chose manque

---

## Points à Améliorer Manuellement

### 1. Renommer PlayerPary.cs → PlayerParry.cs
- ⚠️ Typo detectée
- Renommez le fichier et mettez à jour les imports
- (Impossible à faire automatiquement avec nos outils)

### 2. Refactoriser PlayerController
- Le fichier est trop gros (violation Single Responsibility)
- Diviser en sous-composants:
  - PlayerInput (gestion des inputs)
  - PlayerStateManager (gestion des états)
  - PlayerMovement ✓ (déjà séparé)

### 3. Code Commenté
- Supprimer le code commenté dans GameManager.cs
- Nettoyer les TODOs inutilisés

### 4. Documentation XML
- Ajouter `/// <summary>` aux classes publiques
- Documenter les méthodes complexes

---

## Changements de Comportement

⚠️ **Attention**: Certains changements peuvent affecter le comportement:

1. **SaveManager**: Passe de BinaryFormatter à JSON
   - Les anciennes saves ne seront pas compatibles
   - Vous devez migrer ou réinitialiser les données

2. **Debug.Log supprimé**: PlayerJump
   - Moins de spam dans la console
   - Légère amélioration des performances

3. **Null checks**: Plusieurs composants
   - Seront désactivés automatiquement si références manquent
   - Vérifiez la Console pour les warnings

---

## Résumé des Fichiers Créés

| Fichier | Description |
|---------|-------------|
| [GameConstants.cs](Assets/Scripts/Utils/GameConstants.cs) | Constantes centralisées |
| [JsonSerializationUtility.cs](Assets/Scripts/Utils/JsonSerializationUtility.cs) | Utilitaire JSON |
| [GameRegistry.cs](Assets/Scripts/Manager/GameRegistry.cs) | Singleton pour accès aux GameObjects |

---

## Prochaines Étapes Recommandées

1. ✅ Appliquer ce guide
2. Test en Play Mode
3. Corriger les warnings de la Console
4. Renommer PlayerPary → PlayerParry
5. Ajouter documentation XML aux publics APIs
6. Refactoriser PlayerController (trop gros)
7. Implémenter ScriptableObjects pour les configs

---

**Date**: 17 Mai 2026  
**Auteur**: GitHub Copilot  
**Version**: 1.0
