# H1 – Identificación del Proceso Lento para Análisis de Algoritmos

## 1. Identificación del Proceso Real
**Proceso:** Gestión y Liquidación de Periodos de Lecturas de Alumbrado Público  
**Componente técnico:** Endpoint `GET /periodos` (`AlumbradoPublicoController.cs`) y mapeo de lotes en `FacLoteFechaResolver.cs`.

---

## 2. Descripción del Proceso Lento (Texto Principal)

El proceso seleccionado es la "Gestión y Liquidación de Periodos de Lecturas de Alumbrado Público", iniciado a través del endpoint HTTP GET /periodos en el backend .NET del sistema.
Este proceso es ejecutado de manera continua por los operadores comerciales y supervisores de facturación de la empresa distribuidora de electricidad.
Los usuarios sufren una lentitud severa e inmovilización de la interfaz de usuario cada vez que abren el módulo mensual o intentan auditar los periodos de un suministro.
El sistema administra un volumen considerable compuesto por 52 suministros municipales activos y un histórico de 173.080 lecturas móviles en la base de datos Oracle.
Asimismo, el proceso se vincula con tablas del sistema comercial central ERP que superan los 445.000 lotes de facturación y el millón de medidores registrados.
En mediciones reales en navegador, la consulta con filtro de año y mes tarda 7,49 segundos, mientras que la consulta por un suministro específico tarda 3,10 segundos.
La demora es provocada por dos factores: la ejecución oculta de procedimientos de cálculo masivo previos a la lectura y un problema N+1 durante el mapeo de los datos.
En la base de datos, el cálculo previo obliga a ejecutar escaneos de tabla completa repetitivos (Full Table Scans) sobre las 173.080 filas de lecturas sin índice.
En el backend, el mapeador de objetos AutoMapper dispara decenas de consultas remotas individuales y sincrónicas contra Oracle para resolver una sola fecha.
Se optimizará este proceso este mes porque representa la puerta de entrada operativa del módulo comercial, bloquea al usuario y sobrecarga la base de datos.
Además, ofrece el caso académico ideal para transformar una búsqueda repetitiva de orden cuadrático y de red en un algoritmo de reunión eficiente de orden lineal.

---

## 3. ¿Cómo se busca, ordena o recorre hoy? (¿Uno por uno o todos contra todos?)

El proceso opera actualmente bajo el antipatrón de **búsqueda repetitiva uno por uno (registro por registro)**, tanto en el backend como en la base de datos:

1. **En el Backend C# (`FacLoteFechaResolver`):**
   - **Mecanismo:** Uno por uno.
   - **Explicación:** Una vez obtenida la lista de $P$ periodos desde la base de datos, AutoMapper recorre los elementos secuencialmente. Para cada periodo individual, ejecuta una consulta SQL síncrona independiente a Oracle contra la tabla `FAC_LOTES` (445.909 filas) buscando por ruta, año y mes. Si la consulta devuelve 30 o 52 periodos, se ejecutan 30 o 52 consultas secuenciales a través de la red, acumulando la latencia de transporte de ida y vuelta en cada registro.

2. **En la Base de Datos Oracle (`SET_CONSUMO_NO_MEDIDO_AP`):**
   - **Mecanismo:** Todos contra todos (producto cartesiano de evaluaciones).
   - **Explicación:** Al filtrar por año y mes, un cursor itera los $K$ suministros del mes ($K = 52$). Por cada uno, invoca un procedimiento que ejecuta:
     ```sql
     SELECT SUM(CONSUMO_AJUST) FROM MOVILES.LECTURAS_AP WHERE PERIODO_LECTURAS_AP_ID = :id;
     ```
     Dado que la tabla `LECTURAS_AP` tiene **173.080 filas** y **no tiene índice** sobre `PERIODO_LECTURAS_AP_ID`, cada uno de los 52 suministros recorre secuencialmente la tabla completa de 173.080 registros, evaluando $52 \times 173.080 = 8.999.160$ tuplas.

---

## 4. Estimación de la Complejidad Temporal Actual ($O$) y Justificación

### Variables:
- $P$: Cantidad de periodos mapeados en la página ($P \le 52$).
- $M$: Filas de la tabla de lotes `ERP_ELFEC.FAC_LOTES` ($M = 445.909$).
- $C_{\text{net}}$: Costo de latencia de red y despacho de conexión por consulta ($C_{\text{net}} \approx 60\text{ a }100\text{ ms}$).
- $K$: Cantidad de suministros del mes ($K = 52$).
- $N$: Cantidad de filas históricas en `MOVILES.LECTURAS_AP` ($N = 173.080$).


---

## 5. Muestra de Datos Reales (Exactamente 20 Filas)

Datos reales obtenidos de la tabla `MOVILES.LECTURAS_AP` de la base de datos de producción (`EPROD`), correspondientes al suministro auditado (`PERIODO_LECTURAS_AP_ID = 1530`), ordenados por secuencia física de visita (`NROORDEN`):

| ID | PERIODO_ID | LIBRETA | NROCAJA | ORDEN | IDMEDIDOR | NROSERIE | FECHA_ANT | FECHA_ACT | LEC_ANT | LEC_ACT | CONS_LEI | CONS_AJU | ESTADO |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| 297209 | 1530 | L1 | 1 | 1 | 578138 | `14102074` | 2026-09-18 | 2026-10-17 | 3705 | 4503 | 700 | 835 | 3 |
| 297216 | 1530 | L1 | 4 | 2 | 582169 | `14106105` | 2026-09-19 | 2026-10-17 | 90735 | 90967 | 218 | 260 | 3 |
| 296588 | 1530 | L1 | 7 | 3 | 765346 | `03419607` | 2026-09-19 | 2026-10-17 | 64602 | 65106 | 466 | 556 | 3 |
| 296427 | 1530 | L1 | 10 | 4 | 806484 | `18224575` | 2026-09-19 | 2026-10-17 | 62050 | 63193 | 1059 | 1263 | 3 |
| 296912 | 1530 | L1 | 13 | 5 | 679839 | `17C1051005486` | 2026-09-19 | 2026-10-17 | 14332 | 15668 | 1245 | 1484 | 3 |
| 299185 | 1530 | L1 | 16 | 6 | 460947 | `11113998` | 2026-09-19 | 2026-10-17 | 65675 | 66176 | 463 | 552 | 3 |
| 297977 | 1530 | L1 | 19 | 7 | 899568 | `205824` | 2026-09-19 | 2026-10-17 | 81183 | 81936 | 691 | 824 | 3 |
| 296584 | 1530 | L1 | 22 | 8 | 765342 | `03419603` | 2026-09-19 | 2026-10-17 | 61018 | 61224 | 191 | 228 | 3 |
| 297434 | 1530 | L1 | 25 | 9 | 529693 | `01085710` | 2026-09-19 | 2026-10-17 | 41369 | 42563 | 1105 | 1318 | 3 |
| 297403 | 1530 | L1 | 28 | 10 | 534865 | `13116596` | 2026-09-19 | 2026-10-17 | 21667 | 21893 | 209 | 249 | 3 |
| 297680 | 1530 | L1 | 31 | 11 | 518888 | `0404697` | 2026-09-19 | 2026-10-17 | 19260 | 19936 | 625 | 745 | 3 |
| 297681 | 1530 | L1 | 34 | 12 | 518889 | `03100610` | 2026-09-19 | 2026-10-17 | 12493 | 12907 | 383 | 457 | 3 |
| 297094 | 1530 | L1 | 37 | 13 | 630018 | `15381021199` | 2026-09-19 | 2026-10-17 | 1391 | 1654 | 244 | 291 | 3 |
| 296219 | 1530 | L1 | 38 | 14 | 989760 | `23761022386` | 2026-09-19 | 2026-10-17 | 3228 | 3532 | 280 | 334 | 3 |
| 296564 | 1530 | L1 | 40 | 15 | 775981 | `15373792` | 2026-09-19 | 2026-10-17 | 87541 | 88219 | 627 | 748 | 3 |
| 297190 | 1530 | L1 | 43 | 16 | 621168 | `15381012725` | 2026-09-19 | 2026-10-17 | 49024 | 49356 | 305 | 364 | 3 |
| 297685 | 1530 | L1 | 46 | 17 | 518895 | `11604552` | 2026-09-19 | 2026-10-17 | 93818 | 94712 | 738 | 880 | 3 |
| 297397 | 1530 | L1 | 49 | 18 | 550562 | `13124495` | 2026-09-19 | 2026-10-17 | 3161 | 3505 | 317 | 378 | 3 |
| 297123 | 1530 | L1 | 52 | 19 | 632443 | `15381023624` | 2026-09-19 | 2026-10-17 | 10566 | 11131 | 522 | 622 | 3 |
| 297702 | 1530 | L1 | 55 | 20 | 518939 | `1716541` | 2026-09-19 | 2026-10-17 | 57843 | 58264 | 390 | 465 | 3 |

---
