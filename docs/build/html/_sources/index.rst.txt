Bienvenido a la documentación de VSlices
========================================

Acá tendrás todo lo que necesitas para poder entender y desarrollar con el framework :)

¿Que es VSlices?
----------------

VSlices es un framework flexible orientado al desarrollo web, desarrollado en C#, fuertemente influenciado 
por la programación *orientada a objetos*, la arquitectura *“Vertical Slices”* (De la cual toma el 
nombre) y el patrón Command Query Responsability Segregation (*CQRS*). Tambien tiene una influencia 
menor de la programación *funcional*.

Este framework gira alrededor del concepto de "caso de uso" (o *features*), siendo estos la documentación 
con la cual crearás el código de tú aplicación, ya que representan las *acciones que pueden realizar 
los usuarios de tú sistema*, por lo que, al codificar con VSlices, debes *siempre crear la documentación* 
asociada 
a cada caso.

Esta diseñado para tener baterias incluidas en varios aspectos, por consecuente, te centras en lo que 
**realmente importa**. La implementación por defecto ya  incluye facilidades para la validación de 
contratos y dominio (Empleando *FluentValidation*), junto con el acceso a datos (Empleando *EntityFramework*), 
pero esta abierto a que, si lo necesitas, puedas crear tus propias implementaciones.

¿Quieres apoyar al desarrollo?
------------------------------

Primero que todo, se agracede mucho, puedes donarme un poco de dinero
para mi adicción al café acá, por `paypal <https://paypal.me/enyu20?country.x=CL&locale.x=es_XC>`__

De verdad, muchas gracias por el apoyo :)
 
.. toctree::
	:maxdepth: 2
	:hidden:
	:caption: Antes de iniciar
	
	./antes de iniciar/inicio
	./antes de iniciar/requisitos

.. toctree::
	:maxdepth: 2
	:hidden:
	:caption: Primeros pasos
	
	./primeros pasos/antes de desarrollar
	./primeros pasos/tu primer caso de uso
	./primeros pasos/completando la web app
		
.. toctree::
	:maxdepth: 2
	:hidden:
	:caption: Profundizando en la teoria 
	
	./teoria/vertical slices
	./teoria/cross cutting
	./teoria/cqrs
	./teoria/caso de uso
	./teoria/pipeline
		
.. toctree::
	:maxdepth: 2
	:hidden: 
	:caption: Funcionalidades
	 
	./funcionalidades/funcionalidades
		
.. toctree::
	:maxdepth: 2
	:hidden:
	:caption: Una app completa
	
	./ejemplo completo/antes de desarrollar 