# Architektur-Philosophie & Naming Conventions

Dieses Projekt folgt einer strikten Trennung von Zuständigkeiten. Das Ziel ist eine saubere, modulare Codebasis ohne Race Conditions durch klar definierte Datenflüsse.

## Kernprinzip: Der Datenfluss

Der Ablauf folgt immer einer Einbahnstraße:
**Trigger/Event ➔ Manager / Controller ➔ Operator ➔ Data**

## 1. Data (Der Zustand)

* **Aufgabe:** Speichert ausschließlich den Zustand und die reinen Werte.
* **Regeln:** Absolut "dumm". Enthält keine Geschäftslogik, keine Entscheidungsfindung und keine komplexen Methoden.
* **Namensgebung:** `[System]Data` (z. B. `EnemyGridData`, `HealthData`)

## 2. Operator (Die Logik)

* **Aufgabe:** Führt Aktionen aus und manipuliert die Daten.
* **Regeln:** Trifft keine eigenen Entscheidungen darüber, *wann* etwas passiert. Weiß nur, *wie* eine Aktion technisch durchgeführt wird, sobald er dazu den Befehl erhält.
* **Namensgebung:** `[System]Operator` (z. B. `EnemyGridOperator`, `HealthOperator`)

## 3. Controller (Lokale Steuerung)

* **Aufgabe:** Reagiert auf physische Trigger (Kollisionen, Inputs, Signale) und trifft Entscheidungen für das gebundene Objekt.
* **Regeln:** Ist immer an ein spezifisches Spielobjekt oder eine Node gebunden. Führt Aktionen nicht selbst aus, sondern delegiert sie an Operatoren.
* **Namensgebung:** `[Objekt]Controller` (z. B. `EnemyShipController`, `PlayerMovementController`)

## 4. Manager (Globale Steuerung)

* **Aufgabe:** Steuert systemübergreifende Abläufe, orchestriert Events und verwaltet globale States.
* **Regeln:** Verhält sich wie ein Controller, ist aber niemals an ein einzelnes, physisches Spielobjekt gebunden. Agiert auf einer übergeordneten Systemebene.
* **Namensgebung:** `[System]Manager` (z. B. `EnemySpawnManager`, `GameManager`)

---

### Einordnung am Beispiel: Gegner-Spawn & Raster

1. Der **EnemySpawnManager** registriert, dass eine neue Welle beginnt und entscheidet, einen Jäger zu spawnen.
2. Er ruft den **EnemyGridOperator** auf und fordert eine freie Position an.
3. Der Operator liest die **EnemyGridData**, sucht einen freien Platz, ändert dort den Wert auf "reserviert" und gibt die Koordinaten zurück.
4. Der Manager instanziiert das Schiff an der errechneten Position.
