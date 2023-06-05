# VSlices

Vslices es un framework de alto nivel C# que premia el rapido desarrollo de software, junto con un diseño limpio y pragmatico, fuertemente inspirado en la arquitectura "Vertical Slices" y la programación funcional, pudiendo compatibilizarlo facilmente con Domain Driven Development.

La documentación de uso del software esta en [link](https://vslice-framework.readthedocs.io/es/latest/), desde ese lugar podras ver como aprender e iniciar usandolo, junto con una opcional introducción a como funciona la arquitectura propuesta y la forma de trabajo.

Si encuentras algún problema en la documentación, tienes propuestas de mejora o clarificación, puedes generar un ticket acá [link]

## Camino de desarrollo

Desarrollos propuestos para VSlices
- Para la versión: v1
  - Handlers
    - Implementación usando FluentValidation ✅ 
  - Repositorios
    - Implementación usando EntityFramework ✅ 
  - Pipelines
    - Implementación de equivalentes de "IPipelineBehaviors" de [jbogard/MediatR](https://github.com/jbogard/MediatR) ✅
    - Implementación de un "IPipelineBehavior" de validación de request ✅
  - Documentación 
    - En español ❌
    - En ingles ❌
- Para la versión: v1.1
  - Implementación de un "cross cutting concerns" pipeline para el logging automatico
  - Implementación de un "cross cutting concerns" pipeline para el manejo de excepciones automatico
- Investigación
  - Investigación sobre más "cross cutting concerns" a agregar.
  - Agregar "Streams"
  - "Ahead Of Time" friendly
  - Analyzer que verifique correcta integridad de los casos de uso.
  - En base al diagrama de caso de uso, hacer una herramienta que genere codigo automaticamente.

## Apoyar el desarrollo de VSlices

Primero que todo, se agracede mucho :), puedes donarme un poco de dinero para mi adicción al café acá, 
por [paypal](https://paypal.me/enyu20?country.x=CL&locale.x=es_XC)

## Agradecimientos

- A Alvaro Osorio, por ser una influencia buena en mis primeros años de carrera

## Inspiraciones

La inspiración más grande que tuve en esto, fue la libreria [jbogard/MediatR](https://github.com/jbogard/MediatR). Muchas gracias por hacer una libreria tan versatil.
