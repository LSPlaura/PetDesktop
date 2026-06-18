# Arquitectura de la Capa de Presentación: MVVM y Comportamientos Desacoplados

El proyecto implementa el patrón de diseño arquitectónico **MVVM (Model-View-ViewModel)** con el objetivo primordial de aislar por completo la lógica de negocio de los componentes visuales de la interfaz de usuario.

Para gestionar las interacciones físicas y los eventos del sistema operativo sin corromper este desacoplamiento, se ha adoptado una estrategia basada en la **extensión de Behaviors (Comportamientos)**.

## Pilares de la Arquitectura

### 1. Desacoplamiento Estricto (MVVM)
* **Vista (View):** Definida de manera puramente declarativa en XAML. El archivo de código subyacente (*Code-Behind*) se mantiene libre de lógica procedimental, sirviendo únicamente para la inicialización nativa del componente.
* **Vista-Modelo (ViewModel):** Gobierna el estado y las reglas de la aplicación de forma abstracta, comunicándose con la vista exclusivamente a través de mecanismos de *Data Binding* y comandos (`ICommand`).
* **Modelo (Model):** Representa las entidades puras de C# y la lógica del negocio (ej. el motor de la mascota virtual), sin conocimiento alguno sobre el entorno gráfico.

### 2. Extensión de Behaviors (Comportamientos Personalizados)
En lugar de procesar los eventos nativos del sistema operativo (como gestos del ratón, teclado o arrastre de ventanas) mediante métodos imperativos dentro de la propia vista, la arquitectura delega estas tareas a clases especializadas que extienden de `Behavior<T>`.



#### Beneficios Técnicos:
* **Reutilización de Código:** Los métodos y la lógica de captura de eventos encapsulados en un Behavior se convierten en componentes inyectables. Pueden ser reutilizados en cualquier contenedor o ventana del proyecto simplemente declarándolos en el XAML.
* **Gestión Eficiente del Ciclo de Vida:** Al controlar explícitamente los momentos de acoplamiento (`OnAttached`) y desacoplamiento (`OnDetaching`), se asegura la correcta liberación de recursos en la memoria RAM, eliminando el riesgo de fugas de memoria (*Memory Leaks*) por suscripciones a eventos huérfanos.
* **Facilidad de Mantenimiento:** Si las reglas de interacción con el sistema operativo cambian (por ejemplo, modificar el comportamiento del arrastre o filtrar un clic específico), el cambio se realiza en un único archivo aislado, sin alterar las vistas ni los ViewModels.
* **Código Limpio y Legible:** Al extraer la "fontanería" visual de los archivos de la vista, el proyecto se alinea con el principio de Responsabilidad Única (SOLID), resultando en una base de código más elegante, modular y fácil de auditar.