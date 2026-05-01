---
title: Arquitectura de Unidades - Guía de Implementación
description: Documentación sobre la nueva jerarquía de clases para unidades en Unity
---

# Arquitectura de Unidades - Nueva Jerarquía de Clases

## 📋 Descripción General

Se ha refactorizado la arquitectura de unidades para implementar un sistema de herencia limpio y mantenible. La clase monolítica `abr` ha sido dividida en una jerarquía de clases bien definida.

## 🏗️ Estructura de Herencia

```
Humano (Clase Abstracta Base)
├── Villager
├── Explorer  
└── Warrior (Clase Abstracta)
    ├── Warrior_Mele
    └── Warrior_Distance
```

## 📚 Descripción de Clases

### Humano (Base)
**Archivo:** `Humano.cs`

Clase abstracta base que contiene toda la lógica común de movimiento, animación y manejo de recursos.

**Características:**
- Movimiento con NavMeshAgent y Rigidbody
- Sistema de animación integrado
- Manejo de órdenes de movimiento
- Interacción con recursos (ResourceNode)
- Visualización de dirección (sprite flip)
- Sistema anti-atascos

**Propiedades configurables:**
- `health`: Salud máxima de la unidad
- `speed`: Velocidad de movimiento base
- `stoppingDistance`: Distancia de detención
- `stuckTimeout`: Tiempo antes de cancelar orden si está atascada
- `rbMass`: Masa del Rigidbody
- `rbDrag`: Resistencia del Rigidbody

**Métodos principales:**
- `SetMoveTarget(Vector3, ResourceNode)`: Establecer objetivo de movimiento
- `CheckArrival()`: Verificar si llegó al objetivo
- `UpdateFacing(Vector3)`: Actualizar orientación del sprite

---

### Villager
**Archivo:** `Villager.cs` (reemplaza a `abr`)

Unidad constructora/recolectora especializada en gathering de recursos.

**Características heredadas de Humano:**
- Todo el sistema de movimiento
- Interacción con recursos

**Nuevas características:**
- `harvestSpeed`: Multiplicador de velocidad de recolección
- Método `BoostHarvestSpeed()`: Mejora permanente de velocidad

**Valores por defecto:**
- Health: 60
- Speed: 5

**Uso:**
```csharp
Villager villager = GetComponent<Villager>();
villager.SetMoveTarget(targetPosition, resourceNode);
```

---

### Explorer
**Archivo:** `Explorer.cs`

Unidad de reconocimiento especializada en exploración y detección.

**Características heredadas de Humano:**
- Todo el sistema de movimiento

**Nuevas características:**
- `visionRange`: Rango de detección (25 unidades por defecto)
- `speedMultiplier`: 40% más rápido que Villager
- Modo exploración con escaneo de área
- Detección de enemigos y recursos cercanos
- Métodos: `StartExploration()`, `StopExploration()`, `ScanArea()`

**Valores por defecto:**
- Health: 50
- Speed: 7 (1.4x más que Villager)
- Vision Range: 25

**Uso:**
```csharp
Explorer explorer = GetComponent<Explorer>();
explorer.StartExploration();
explorer.UpgradeVisionRange(5f);
```

---

### Warrior (Base para Guerreros)
**Archivo:** `Warrior.cs`

Clase abstracta base para unidades de combate. Define sistema de combate común.

**Características heredadas de Humano:**
- Todo el sistema de movimiento

**Nuevas características:**
- `attackPower`: Poder de ataque base
- `attackRange`: Rango de ataque
- `attackCooldown`: Tiempo entre ataques
- `armor`: Reducción de daño recibido
- Sistema de targeting

**Métodos:**
- `SetAttackTarget(GameObject)`: Establecer objetivo
- `TakeDamage(float)`: Recibir daño (considerando armadura)
- `Die()`: Destruir unidad al morir

**Valores por defecto:**
- Health: 100
- Speed: 4.5
- Attack Power: 25
- Attack Range: 1.5
- Armor: 5

**Nota:** No se puede instanciar directamente. Usa Warrior_Mele o Warrior_Distance.

---

### Warrior_Mele
**Archivo:** `Warrior_Mele.cs`

Guerrero especializado en combate cuerpo a cuerpo.

**Características específicas:**
- `meleeDamageMultiplier`: Multiplicador de daño (1.5x)
- `knockbackForce`: Fuerza de empuje al atacar
- Efectos visuales de ataque
- Daño aumentado con multiplicador

**Valores por defecto:**
- Health: 120
- Speed: 4.5
- Attack Power: 30
- Attack Range: 1.2
- Armor: 8

**Métodos:**
- `PerformAttack()`: Ejecutar ataque con knockback
- `UpgradeMeleeDamage(float)`: Aumentar daño permanentemente

**Uso:**
```csharp
Warrior_Mele warrior = GetComponent<Warrior_Mele>();
warrior.SetAttackTarget(enemyGameObject);
warrior.UpgradeMeleeDamage(10f);
```

---

### Warrior_Distance
**Archivo:** `Warrior_Distance.cs`

Guerrero especializado en combate a distancia (arquero).

**Características específicas:**
- `rangedDamageMultiplier`: Multiplicador de daño (1.2x)
- `projectileSpeed`: Velocidad del proyectil
- `projectilePrefab`: Prefab del proyectil a disparar
- `shootPoint`: Punto de origen del disparo
- `maxRange`: Rango máximo (15 unidades)

**Valores por defecto:**
- Health: 80
- Speed: 5
- Attack Power: 25
- Attack Range: 10
- Max Range: 15
- Armor: 3

**Métodos:**
- `PerformAttack()`: Disparar proyectil
- `UpgradeRange(float)`: Aumentar rango máximo
- `UpgradeProjectileSpeed(float)`: Aumentar velocidad del proyectil

**Uso:**
```csharp
Warrior_Distance archer = GetComponent<Warrior_Distance>();
archer.SetAttackTarget(enemyGameObject);
archer.UpgradeRange(5f);
```

---

### Projectile
**Archivo:** `Projectile.cs`

Script auxiliar para proyectiles disparados por Warrior_Distance.

**Características:**
- Auto-destrucción después de impacto o timeout
- Aplicación de daño al impacto
- No impacta con el disparador

**Métodos:**
- `SetDamage(float)`: Establecer daño del proyectil
- `SetShooter(Warrior)`: Identificar al disparador
- `SetLifetime(float)`: Establecer duración máxima

---

## 🔄 Compatibilidad Hacia Atrás

Los siguientes archivos legacy se mantienen para compatibilidad:
- `abr.cs`: Alias de Villager
- `unit_villager.cs`: Hereda de Villager
- `unit_warrior.cs`: Implementa Warrior (genérico)
- `unit_warrior_mele.cs`: Hereda de Warrior_Mele
- `unit_warrior_distance.cs`: Hereda de Warrior_Distance
- `Unit_explorer.cs`: Hereda de Explorer

**Se recomienda migrar a los nuevos nombres de clase.**

---

## 💡 Ejemplos de Uso

### Crear un Villager en una escena
```csharp
// Agregar componente a un GameObject
gameObject.AddComponent<Villager>();
```

### Mover una unidad a un recurso
```csharp
Humano unit = GetComponent<Humano>();
ResourceNode resource = resourceObject.GetComponent<ResourceNode>();
unit.SetMoveTarget(resource.transform.position, resource);
```

### Sistema de combate
```csharp
// Guerrero melee atacando
Warrior_Mele meleeWarrior = GetComponent<Warrior_Mele>();
meleeWarrior.SetAttackTarget(enemyUnit.gameObject);

// Guerrero a distancia atacando
Warrior_Distance rangedWarrior = GetComponent<Warrior_Distance>();
rangedWarrior.SetAttackTarget(enemyUnit.gameObject);
```

### Exploración
```csharp
Explorer scout = GetComponent<Explorer>();
scout.StartExploration();
```

---

## 🚀 Mejoras Futuras

Posibles extensiones de la arquitectura:
1. **Sistema de skills**: Agregar métodos virtuales para habilidades especiales
2. **Animaciones sincronizadas**: Mejorar las transiciones de animación
3. **Sistema de upgrades**: Crear árbol de mejoras por tipo de unidad
4. **IA de comportamiento**: Agregar FSM para patrones de comportamiento
5. **Multiplayer mejorado**: Sincronización de red más robusta
6. **Partículas y efectos**: Sistema de efectos visuales por tipo de ataque

---

## 📝 Notas Importantes

1. **No instanciar clases abstractas**: Humano y Warrior no se pueden instanciar directamente.
2. **Configuración en Inspector**: Todos los parámetros son públicos y configurables en el Inspector de Unity.
3. **Orden de herencia**: Las clases derivadas deben llamar a `base.Start()`, `base.Update()`, etc.
4. **Rigidbody requerido**: Todas las unidades necesitan un componente Rigidbody.
5. **NavMeshAgent requerido**: Todas las unidades necesitan un NavMeshAgent para movimiento.
6. **Animator opcional**: Las animaciones son opcionales pero recomendadas.

---

## 📞 Soporte

Para preguntas sobre la arquitectura, consultar:
- Comentarios en el código de cada clase
- Esta documentación
- Ejemplos en el código existente
