# Plan de refactorisation — Projet Cube

Ce document liste de manière exhaustive les refactorings recommandés pour le code présent dans `Assets/Scripts`. Chaque item contient : priorité, fichiers concernés, problème constaté, solution proposée et estimation approximative.

---

**Priorités :**
- Critique = affecte stabilité/compilation ou provoque bugs visibles
- Important = maintenabilité / performance
- Amélioration = qualité/consistance / style

---

## 1) Critique (à traiter en priorité)

- Remplacer `BinaryFormatter` (incompatible .NET8)
  - Fichiers : [Assets/Scripts/Manager/SaveManager.cs](Assets/Scripts/Manager/SaveManager.cs)
  - Problème : API dépréciée, risque de sécurité et incompatibilité.
  - Solution : utiliser `JsonUtility` ou `Newtonsoft.Json` et migrer les données (nouveau format JSON). Ajouter versioning de save.
  - Estimation : 1-2h

- Corriger l'ordre des composants de Quaternion/Vector4
  - Fichiers : [Assets/Scripts/Manager/SaveManager.cs](Assets/Scripts/Manager/SaveManager.cs)
  - Problème : `new Quaternion(w,x,y,z)` utilisé au lieu de `(x,y,z,w)`, données de rotation corrompues.
  - Solution : corriger constructeur et ajouter tests de roundtrip.
  - Estimation : 30min

- Corriger `SoundManager` Lerp / null-reference
  - Fichiers : [Assets/Scripts/Manager/SoundManager.cs](Assets/Scripts/Manager/SoundManager.cs)
  - Problème : `Mathf.Lerp(min, min, ...)` et `FindGameObjectWithTag` sans null-check.
  - Solution : corriger lerp; utiliser `GameRegistry` ou vérif null; supprimer Debug.Log en Update.
  - Estimation : 30min

- Raycasts mal positionnés dans `PlayerController` (rays montent)
  - Fichiers : [Assets/Scripts/Player/PlayerController.cs](Assets/Scripts/Player/PlayerController.cs)
  - Problème : offsets Y des rays à `0.5f` -> certaines rays pointent vers le haut.
  - Solution : recalculer origins (center + corners) et ajouter Gizmos pour debug; utiliser `Physics.SphereCast` / `CheckSphere` selon besoin.
  - Estimation : 1h

- Movement non deterministe : `CharacterController.Move` appelé dans `Update`
  - Fichiers : [Assets/Scripts/Player/PlayerMovement.cs](Assets/Scripts/Player/PlayerMovement.cs)
  - Problème : mouvement dépend du framerate; gravity / platform movement appelés ailleurs.
  - Solution : centraliser la physique en `FixedUpdate()` (ou utiliser `PlayerPhysicsHandler`) ; lire input dans `Update` mais appliquer mouvement en `FixedUpdate`.
  - Estimation : 2-4h

- Plateformes et sticking (double système : parent transform + CharacterController)
  - Fichiers : [Assets/Scripts/Props/PlatformeController.cs](Assets/Scripts/Props/PlatformeController.cs), PlayerMovement, PlayerPhysicsHandler
  - Problème : plateforme se déplace via `CharacterController.Move` sur la plateforme, joueur est parenté au transform mais utilise un `CharacterController` indépendamment -> conflits et jitter.
  - Solution :
    - Option A (recommandée) : plateformes déplacées via `transform.position` (changer pour delta position explicite) et transmettre le delta au `PlayerPhysicsHandler` qui l'applique lors du Move. Ne pas parent/supprimer parent ; gérer stick via velocity addition.
    - Option B : si plateformes doivent utiliser CC, ne pas parent/faire en sorte que le joueur recoive et ajoute la velocity de la plateforme dans son Move.
  - Estimation : 3-6h

---

## 2) Important (bonne valeur ajoutée)

- Centraliser les constantes et les tags
  - Fichiers : `Assets/Scripts/Utils/GameConstants.cs` (créer/compléter)
  - Problème : magic strings (`"Player"`, `"ProjectileParry"`, etc.) dispersées
  - Solution : utiliser constantes et énumérations; mettre à jour tous les usages.
  - Estimation : 1h

- Remplacer `GameObject.Find()` par `GameRegistry`/DI
  - Fichiers : multiples (DiamondController, SpeedRunManager, TrailRenderer, PlayerController, etc.)
  - Problème : appels fragiles et coûteux.
  - Solution : créer `GameRegistry` (singleton léger) ou injecter via l'inspector; remplacer usages.
  - Estimation : 2-4h

- Cache GetComponent et éviter répétitions
  - Fichiers : multiples
  - Problème : `GetComponent`/`Find` répétés dans Update
  - Solution : mettre en cache en `Start/Awake`.
  - Estimation : 1-2h

- Supprimer `Debug.Log` dans `Update()` et `FixedUpdate()` coûteux
  - Fichiers : PlayerJump, autres
  - Estimation : 30min

- Consolider les méthodes de UI fade (dupliquées)
  - Fichiers : `Assets/Scripts/Manager/Functions.cs`, `Assets/Scripts/UI/UIFunctions.cs`
  - Problème : duplication de Fade/Show/Hide
  - Solution : garder implémentation unique (Utils.Functions) et modifier usages; corriger sens des paramètres.
  - Estimation : 1h

---

## 3) Améliorations (qualité & maintenance)

- Refactor `PlayerController` (trop de responsabilités)
  - Séparer : input routing, état (Actions bitflag), logique de collision/décès, orchestration des abilities (jump, parry, teleport)
  - Fichiers : `PlayerController.cs`, `PlayerTeleport.cs`, `PlayerParry` (ou `PlayerPary`)...
  - Estimation : 6-12h (par étapes)

- Unifier la gestion des entrées
  - Fichiers : tous dans `Player/`
  - Problème : création multiple d'instances `Control` dans différents scripts
  - Solution : centraliser `Control` dans `PlayerController` et exposer les `InputAction` aux modules
  - Estimation : 2-4h

- Implémenter ScriptableObjects pour configuration (vitesse, gravité, timers)
  - Avantages : tuning en editor sans recompiler
  - Estimation : 3-6h

- Ajouter documentation XML et commentaires pour API publique
  - Fichiers : Managers publics, Player API
  - Estimation : 2-4h

---

## 4) Sécurité & Robustesse

- Ajouter null-checks et fallbacks lors des `Find`/GetComponent
- Ajout de `TryGetComponent` lorsque pertinent
- Ajouter protections pour l'accès aux `saveManager.dataPlayer` (null checks)

---

## 5) Tests & Profiling

- Ajouter scènes de test pour : plateforme mobile + joueur collé; sauvegarde roundtrip; parry/projectile behaviour
- Utiliser Unity Profiler pour vérifier allocations (GC) causées par `Debug.Log`, allocations temporaires, `DOTween` usages

---

## 6) Liste détaillée par fichier (sélection non exhaustive)

- `Assets/Scripts/Manager/SaveManager.cs`
  - Remplacer `BinaryFormatter` par JSON; corriger Quaternion/Vector4 order; ajouter version de save

- `Assets/Scripts/Player/PlayerController.cs`
  - Corriger raycasts (offsets); séparer responsibilities; utiliser `GameRegistry` pour references; éviter `GameObject.Find` dans Death()

- `Assets/Scripts/Player/PlayerMovement.cs`
  - Lire input dans `Update()` mais appliquer mouvement via `PhysicsHandler` en `FixedUpdate()`; retirer parent transform comme mécanisme principal pour sticking

- `Assets/Scripts/Player/PlayerJump.cs` (ou `PlayerJumpNew.cs`)
  - Utiliser le PhysicsHandler; appliquer gravity en FixedUpdate; supprimer logs inutile

- `Assets/Scripts/Props/PlatformeController.cs`
  - Remplacer `CharacterController.Move(delta)` par `transform.position = nextPos` ou calculer delta et exposer `delta` via événement pour que le player l'ajoute pendant FixedUpdate. Ne pas parent/supprimer parent. Ajouter option pour transmettre velocity.

- `Assets/Scripts/Props/DiamondController.cs` / `DiamondManager.cs`
  - Utiliser `GameRegistry` et cache; supprimer `DOTween` chain duplication en utilitaire

- `Assets/Scripts/Effects/TrailRenderer.cs`
  - Récupérer `playerTransform` via `GameRegistry`; éviter allocations par frame

- `Assets/Scripts/Manager/Functions.cs` & `Assets/Scripts/UI/UIFunctions.cs`
  - Conserver une seule implémentation asynchrone et documenter comportement; corriger sens des fades

- Divers: renommer `PlayerPary.cs` → `PlayerParry.cs` (typo), standardiser noms (PascalCase)

---

## 7) Plan d'exécution recommandé (ordre proposé)

1. Sauvegarde & quaternions fixes (critique)  
2. Centraliser constantes (`GameConstants`) + créer `GameRegistry`  
3. Remplacer `GameObject.Find()` par `GameRegistry`  
4. Refactorer mouvement du joueur (PhysicsHandler/FixedUpdate)  
5. Fix plateformes : transmettre delta → appliquer au joueur (stick)  
6. Consolider UI + supprimer logs  
7. Tests, profiler, ajustements

---

## 8) Estimation globale

- Effort total approximatif : 2–4 jours (dépend du scope exact et tests)  
- Livrables attendus : code refactoré, tests manuels, notes de migration, mise à jour des scenes (GameRegistry attaché et configuré)

---

Si tu veux, je peux :
- Générer automatiquement un PR avec les premières modifications (save + constants + GameRegistry)
- Commencer directement le refactor du système de mouvement et des plateformes

Dis-moi quelle action prioriser et je lance les modifications.