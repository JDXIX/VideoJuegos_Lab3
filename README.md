# Lab 3 - Aplicación Unity 3D con Base de Datos

## Implementación de una aplicación con base de datos: integración, sincronización y lógica compartida

Este repositorio contiene el desarrollo de un laboratorio práctico en Unity 3D, cuyo objetivo es implementar una aplicación interactiva conectada a una base de datos MySQL/MariaDB mediante una API local desarrollada en PHP.

El proyecto permite controlar un personaje 3D, recolectar monedas, sumar puntuación, guardar el estado del jugador y cargar nuevamente la información almacenada en la base de datos.

---

## Información general

| Dato               | Descripción                               |
| ------------------ | ----------------------------------------- |
| Proyecto           | JuegoConBaseDeDatos                       |
| Motor              | Unity 6.3 LTS                             |
| Lenguaje principal | C#                                        |
| Backend local      | PHP                                       |
| Base de datos      | MySQL / MariaDB                           |
| Servidor local     | XAMPP                                     |
| Comunicación       | UnityWebRequest                           |
| Tipo de juego      | Prototipo 3D con persistencia de datos    |
| Autor              | Jordy Quimbita                            |
| Institución        | Universidad de las Fuerzas Armadas - ESPE |

---

## Objetivo del proyecto

Implementar una aplicación 3D en Unity que permita demostrar la integración entre un videojuego y una base de datos local, utilizando una arquitectura cliente-servidor basada en:

```text
Unity 3D
   ↓
UnityWebRequest
   ↓
PHP
   ↓
MySQL / MariaDB
```

El jugador puede moverse dentro de una escena 3D, recolectar monedas, modificar su puntuación, guardar su progreso y cargar los datos previamente almacenados.

---

## Funcionalidades implementadas

El proyecto incluye las siguientes funcionalidades:

* Movimiento del personaje en entorno 3D.
* Personaje visual importado en formato FBX.
* Escenario 3D con terreno.
* Monedas recolectables.
* Sistema de puntuación.
* Sistema de vida.
* Sistema de nivel.
* Guardado manual de partida.
* Carga manual de partida.
* Auto-guardado por tiempo.
* Auto-guardado por distancia recorrida.
* Persistencia de datos en base de datos.
* API PHP para consultas `GET` y `POST`.
* Comunicación HTTP desde Unity mediante `UnityWebRequest`.
* Visualización de datos en pantalla mediante interfaz básica.
* Verificación de registros desde phpMyAdmin.

---

## Tecnologías utilizadas

### Unity

Unity se utilizó como motor principal para construir la escena 3D, controlar el personaje, detectar colisiones y ejecutar la lógica del juego.

Componentes principales usados en Unity:

* GameObject
* Transform
* Rigidbody
* Capsule Collider
* Sphere Collider
* Terrain
* Scripts C#
* UnityWebRequest
* Input System
* OnTriggerEnter
* OnGUI

---

### XAMPP

XAMPP se utilizó para levantar el servidor local. En este proyecto se usaron los siguientes servicios:

* Apache
* MySQL / MariaDB
* PHP
* phpMyAdmin

Debido a que el puerto 80 estaba ocupado en el equipo de desarrollo, Apache fue configurado para ejecutarse en el puerto:

```text
8080
```

Por esa razón, la URL base del proyecto es:

```text
http://localhost:8080/
```

---

### PHP

PHP se utilizó para crear una API local llamada `game.php`, ubicada en:

```text
C:\xampp\htdocs\Juego\game.php
```

Este archivo permite:

* Recibir solicitudes `GET` desde Unity.
* Recibir solicitudes `POST` con datos JSON.
* Consultar registros existentes del jugador.
* Guardar o actualizar partidas.
* Responder a Unity en formato JSON.

---

### Base de datos

La base de datos utilizada se llama:

```sql
juego_bd
```

La tabla principal se llama:

```sql
partidas_guardadas
```

---

## Estructura general del proyecto

```text
JuegoConBaseDeDatos/
│
├── Assets/
│   ├── Modelo/
│   │   ├── Mario/
│   │   │   ├── Mario.FBX
│   │   │   └── Animations/
│   │   │
│   │   └── Moneda/
│   │       └── moneda de mario.fbx
│   │
│   ├── Scenes/
│   │   └── Nivel1.unity
│   │
│   └── Scripts/
│       ├── GameData.cs
│       ├── DataBaseManager.cs
│       ├── ControladorMario.cs
│       └── Moneda.cs
│
├── ProjectSettings/
├── Packages/
└── README.md
```

---

## Estructura del servidor local

```text
C:\xampp\htdocs\Juego\
└── game.php
```

La API se prueba desde el navegador con:

```text
http://localhost:8080/Juego/game.php?jugador_id=test001
```

Respuesta esperada:

```json
{
  "jugador_id": "test001",
  "jugador_nombre": "NuevoJugador",
  "puntuacion": 0,
  "posicion_x": 0,
  "posicion_z": 0,
  "vida": 100,
  "nivel": 1
}
```

---

## Script SQL de la base de datos

Para crear la tabla del proyecto se utilizó el siguiente script:

```sql
CREATE TABLE partidas_guardadas (
    id INT AUTO_INCREMENT PRIMARY KEY,
    jugador_id VARCHAR(50) UNIQUE NOT NULL,
    jugador_nombre VARCHAR(50) NOT NULL,
    puntuacion INT DEFAULT 0,
    posicion_x FLOAT DEFAULT 0,
    posicion_z FLOAT DEFAULT 0,
    vida INT DEFAULT 100,
    nivel INT DEFAULT 1,
    ultima_actualizacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);
```

---

## Descripción de los scripts principales

### GameData.cs

Este script define la estructura de datos que se enviará y recibirá desde la API PHP.

Campos principales:

```text
jugador_id
jugador_nombre
puntuacion
posicion_x
posicion_z
vida
nivel
```

Su función principal es representar los datos del jugador en formato serializable para convertirlos a JSON.

---

### DataBaseManager.cs

Este script administra la comunicación entre Unity y el servidor PHP.

Funciones principales:

* Guardar partida.
* Cargar partida.
* Convertir datos a JSON.
* Enviar datos mediante método `POST`.
* Consultar datos mediante método `GET`.
* Recibir respuestas del servidor.
* Mostrar mensajes de depuración en consola.

URL utilizada:

```csharp
http://localhost:8080/Juego/game.php
```

Cuando el guardado funciona correctamente, la consola muestra:

```text
Código HTTP: 200
Respuesta servidor: {"success":true,"mensaje":"Partida guardada correctamente"}
Partida guardada correctamente.
```

---

### ControladorMario.cs

Este script controla la lógica principal del personaje.

Funciones implementadas:

* Movimiento con teclado.
* Generación o recuperación del ID del jugador.
* Carga inicial de partida.
* Guardado manual.
* Carga manual.
* Auto-guardado por tiempo.
* Auto-guardado por distancia.
* Suma de puntos.
* Daño recibido.
* Curación.
* Visualización del estado del jugador.

Controles disponibles:

| Tecla                | Acción                    |
| -------------------- | ------------------------- |
| W / Flecha arriba    | Avanzar                   |
| S / Flecha abajo     | Retroceder                |
| A / Flecha izquierda | Moverse a la izquierda    |
| D / Flecha derecha   | Moverse a la derecha      |
| G                    | Guardar partida           |
| L                    | Cargar partida            |
| C                    | Mostrar estado en consola |
| X                    | Sumar puntos              |
| Z                    | Recibir daño              |
| R                    | Curar vida                |

---

### Moneda.cs

Este script controla el comportamiento de las monedas recolectables.

Funciones principales:

* Rotación constante de la moneda.
* Movimiento de flotación.
* Detección de contacto con el jugador.
* Suma de puntos al jugador.
* Guardado automático al recolectar.
* Destrucción de la moneda recolectada.

Cada moneda suma:

```text
10 puntos
```

---

## Configuración de la escena

La escena principal se llama:

```text
Nivel1
```

La escena contiene los siguientes objetos principales:

```text
Nivel1
├── Main Camera
├── Directional Light
├── Terreno
├── Mario
│   └── Mario
├── DataBaseManager
├── Moneda_01
├── Moneda_02
├── Moneda_03
└── Moneda_04
```

---

## Personaje

El personaje principal está compuesto por:

* Un objeto padre llamado `Mario`.
* Un modelo visual FBX como hijo.
* Un `Rigidbody`.
* Un `Capsule Collider`.
* El script `ControladorMario`.

El modelo FBX se usa únicamente como representación visual.
La cápsula funciona como cuerpo físico del jugador.

Configuración recomendada del `Rigidbody`:

```text
Use Gravity: desactivado
Is Kinematic: desactivado
Freeze Rotation X: activado
Freeze Rotation Z: activado
```

---

## Monedas

Cada moneda contiene:

* Modelo FBX de moneda.
* Sphere Collider.
* Opción `Is Trigger` activada.
* Script `Moneda.cs`.

Configuración recomendada:

```text
Sphere Collider
Is Trigger: true
Radius: 0.5
```

Cuando Mario toca una moneda:

1. Se ejecuta `OnTriggerEnter`.
2. Se suman puntos al jugador.
3. Se guarda la partida.
4. La moneda desaparece de la escena.

---

## Configuración de Apache

En este proyecto Apache se ejecuta en el puerto:

```text
8080
```

Por lo tanto, las rutas utilizadas son:

```text
http://localhost:8080/
http://localhost:8080/phpmyadmin/
http://localhost:8080/Juego/game.php
```

Si Apache usa el puerto 80 en otro equipo, la URL podría cambiar a:

```text
http://localhost/Juego/game.php
```

En ese caso, también se debe actualizar la URL en `DataBaseManager.cs`.

---

## Instalación y ejecución del proyecto

### 1. Clonar el repositorio

```bash
git clone URL_DEL_REPOSITORIO
```

Entrar a la carpeta del proyecto:

```bash
cd JuegoConBaseDeDatos
```

---

### 2. Abrir el proyecto en Unity

1. Abrir Unity Hub.
2. Seleccionar `Add project from disk`.
3. Buscar la carpeta del proyecto.
4. Abrirlo con Unity 6.3 LTS o una versión compatible.

---

### 3. Configurar XAMPP

1. Abrir XAMPP Control Panel.
2. Iniciar Apache.
3. Iniciar MySQL.
4. Verificar que ambos servicios estén en verde.

Si se usa puerto 8080, ingresar a:

```text
http://localhost:8080/phpmyadmin/
```

---

### 4. Crear la base de datos

Desde phpMyAdmin crear la base:

```sql
juego_bd
```

Luego ejecutar el script de creación de la tabla `partidas_guardadas`.

---

### 5. Configurar la API PHP

Crear la carpeta:

```text
C:\xampp\htdocs\Juego
```

Dentro de esa carpeta colocar el archivo:

```text
game.php
```

Probar la API desde el navegador:

```text
http://localhost:8080/Juego/game.php?jugador_id=test001
```

---

### 6. Ejecutar el juego

1. Abrir la escena `Nivel1`.
2. Verificar que `DataBaseManager` tenga la URL correcta.
3. Presionar `Play`.
4. Mover a Mario con WASD o flechas.
5. Recolectar monedas.
6. Guardar con `G`.
7. Cargar con `L`.
8. Verificar los datos en phpMyAdmin.

---

## Pruebas realizadas

Durante el laboratorio se realizaron las siguientes pruebas:

### Prueba de carga inicial

Al iniciar el juego, Unity consulta la API y carga los datos del jugador.

Resultado esperado:

```text
Respuesta carga: {...}
Partida cargada - Puntos: 0, Vida: 100
```

---

### Prueba de movimiento

Se verificó que el personaje pueda desplazarse usando:

```text
WASD
Flechas direccionales
```

---

### Prueba de guardado automático

El sistema guarda automáticamente por:

* Tiempo.
* Distancia recorrida.

Resultado esperado en consola:

```text
Partida guardada (Auto-guardado por tiempo)
Partida guardada (Auto-guardado por distancia)
Código HTTP: 200
Partida guardada correctamente.
```

---

### Prueba de recolección de monedas

Al tocar una moneda, se incrementa la puntuación.

Resultado esperado:

```text
+10 puntos. Total: 10
Partida guardada (Suma de puntos)
```

---

### Prueba de guardado manual

Al presionar `G`, se guarda la partida.

Resultado esperado:

```text
Partida guardada (Manual)
Código HTTP: 200
Respuesta servidor: {"success":true,"mensaje":"Partida guardada correctamente"}
```

---

### Prueba de carga manual

Al presionar `L`, se carga la partida desde la base de datos.

Resultado esperado:

```text
Respuesta carga: {...}
Partida cargada - Puntos: 60, Vida: 100
```

---

### Prueba en phpMyAdmin

Se verificó que los datos se almacenen correctamente en:

```text
juego_bd → partidas_guardadas
```

Campos validados:

```text
jugador_id
jugador_nombre
puntuacion
posicion_x
posicion_z
vida
nivel
ultima_actualizacion
```

---

## Resultado final

El laboratorio se completó correctamente.
El videojuego permite mover al personaje, recolectar monedas, sumar puntos, guardar y cargar datos persistentes desde una base de datos local.

La integración funciona mediante una arquitectura cliente-servidor simple:

```text
Unity 3D como cliente
PHP como API intermedia
MySQL / MariaDB como almacenamiento
```

---

## Evidencias funcionales

Durante la ejecución se observaron mensajes como:

```text
=== INICIANDO JUEGO ===
ID existente: 3b8117ac-381f-4e08-82d0-7d493e8c6129
Respuesta carga: {...}
Partida cargada - Puntos: 60, Vida: 100
Código HTTP: 200
Respuesta servidor: {"success":true,"mensaje":"Partida guardada correctamente"}
Partida guardada correctamente.
+10 puntos. Total: 60
```

Estos mensajes demuestran que:

* Unity se conecta correctamente con PHP.
* PHP responde correctamente.
* La base de datos almacena la información.
* El sistema de monedas suma puntos.
* La carga de datos funciona correctamente.

---

## Recomendaciones para el repositorio

Se recomienda no subir al repositorio las siguientes carpetas generadas automáticamente por Unity:

```text
Library/
Temp/
Obj/
Build/
Builds/
Logs/
UserSettings/
.vs/
```

Ejemplo recomendado de `.gitignore` para Unity:

```gitignore
[Ll]ibrary/
[Tt]emp/
[Oo]bj/
[Bb]uild/
[Bb]uilds/
[Ll]ogs/
[Uu]ser[Ss]ettings/
.vs/
*.csproj
*.sln
*.user
*.pidb
*.booproj
*.svd
*.pdb
*.mdb
sysinfo.txt
```

---

## Estado del proyecto

Estado actual:

```text
Funcional
```

Componentes completados:

* Servidor local configurado.
* Base de datos creada.
* API PHP funcional.
* Proyecto Unity 3D creado.
* Scripts implementados.
* Personaje integrado.
* Monedas recolectables integradas.
* Guardado y carga funcionando.
* Persistencia verificada en phpMyAdmin.

---

## Posibles mejoras futuras

Algunas mejoras opcionales para futuras versiones son:

* Agregar animaciones al personaje.
* Implementar pantalla de inicio.
* Crear menú de pausa.
* Agregar efectos de sonido.
* Mostrar puntuación con UI moderna.
* Implementar varios niveles.
* Crear sistema de ranking.
* Guardar monedas recolectadas.
* Agregar autenticación de usuarios.
* Publicar el backend en un servidor externo.

---

## Referencias

* Unity Technologies, “UnityWebRequest.”
  https://docs.unity3d.com/2021.3/Documentation/Manual/UnityWebRequest.html

* Unity Technologies, “Interacting with web servers.”
  https://docs.unity3d.com/6000.1/Documentation/Manual/web-request.html

* Unity Technologies, “Collider.OnTriggerEnter.”
  https://docs.unity3d.com/6000.4/Documentation/ScriptReference/Collider.OnTriggerEnter.html

* Apache Friends, “XAMPP Installers and Downloads.”
  https://www.apachefriends.org/

* GitHub Docs, “About READMEs.”
  https://docs.github.com/en/repositories/managing-your-repositorys-settings-and-features/customizing-your-repository/about-readmes

---

## Autor

**Jordy Quimbita**
Universidad de las Fuerzas Armadas - ESPE
Latacunga - Ecuador
2026
