# Documentación: Sistema de Estadísticas de Gastos

## Descripción General

Se ha añadido un sistema completo de cálculo de estadísticas que incluye:

1. **Media de Gastos**: Calcula el promedio de todos los gastos mostrados en la DGV
2. **Top 3 Tipos**: Identifica los 3 tipos de gasto con mayor cantidad total

## Componentes Implementados

### 1. Programa COBOL (`estadi.cob`)

Programa independiente que realiza los cálculos de estadísticas:

**Funcionalidades:**
- Lee los datos de gastos desde un archivo temporal
- Calcula la media aritmética de todos los gastos
- Agrupa gastos por tipo
- Ordena los tipos por total de gasto de mayor a menor
- Genera un archivo `.estad` con los resultados

**Archivos de entrada/salida:**
- Entrada: Archivo de datos de gastos
- Salida: Archivo `[nombre_archivo].estad` con los resultados

**Formato de salida:**
```
[media]
[tipo1]|[total1]
[tipo2]|[total2]
[tipo3]|[total3]
```

### 2. Clase EstadisticaResultado (`EstadisticaResultado.cs`)

Define las estructuras de datos para transportar los resultados:

```csharp
public class EstadisticaResultado
{
    public decimal MediaGastos { get; set; }
    public List<Top3Gasto> Top3Gastos { get; set; }
}

public class Top3Gasto
{
    public string Tipus { get; set; }
    public decimal TotalGasto { get; set; }
}
```

### 3. Método en Despesa.cs: `CalcularEstadisticas`

```csharp
public static EstadisticaResultado CalcularEstadisticas(List<Despesa> dades)
```

**Parámetros:**
- `dades`: Lista de objetos Despesa con los gastos a procesar

**Retorno:**
- Objeto `EstadisticaResultado` con media y top 3

**Funcionamiento:**
- Procesa los datos directamente desde C# sin necesidad de archivos intermedios
- Calcula la media de costos
- Agrupa por tipo y ordena por total
- Toma los primeros 3 resultados

### 4. ReportForm.cs Actualizado

El método `LoadReport` ahora:
1. Calcula las estadísticas usando `Despesa.CalcularEstadisticas()`
2. Crea tres DataSources para el informe:
   - `DataSet1`: Datos de gastos individuales
   - `EstadisticasDataSet`: Media de gastos
   - `Top3DataSet`: Top 3 tipos de gasto

### 5. Informe RDLC Actualizado

El archivo `ReportDespeses.rdlc` ahora incluye:

**Nueva Sección de Estadísticas:**
- Título "ESTADÍSTICAS"
- Etiqueta "Media de Gastos:" con el valor calculado en azul
- Sección "Top 3 Tipos con Mayor Gasto"
- Tabla mostrando:
  - Nombre del tipo
  - Total de gasto en formato moneda (2 decimales)

**Estilos:**
- Fondo azul (#0070C0) para encabezados de tabla
- Bordes grises para celdas
- Formato monetario automático para valores

## Flujo de Uso

1. **Usuario carga datos en la DGV** (DataGridView)
2. **Usuario selecciona "Mostrar Informe"**
3. **C# calcula estadísticas** en tiempo real usando `CalcularEstadisticas()`
4. **ReportForm crea los DataSources** con datos y estadísticas
5. **RDLC renderiza el informe** mostrando:
   - Gráficos originales de costos
   - Nueva sección de estadísticas con media y top 3

## Notas Técnicas

- **Lenguaje de Cálculo**: C# (LINQ)
- **Programa COBOL**: Disponible como alternativa independiente (`estadi.cob`)
- **Almacenamiento**: Sin dependencias de archivos externos en el flujo principal
- **Rendimiento**: Los cálculos se realizan en memoria
- **Precisión Decimal**: Se mantienen 2 decimales en todos los cálculos

## Compilación del Programa COBOL

Si se desea compilar `estadi.cob`:

```bash
cobol estadi.cob
```

Esto generará el ejecutable `estadi.exe` que puede ser llamado desde C# si se requiere procesamiento externo.

## Ejemplo de Uso en Código

```csharp
// Obtener datos filtrados
var datosFiltrados = dgvDespeses.DataSource as List<Despesa>;

// Calcular estadísticas
var estadisticas = Despesa.CalcularEstadisticas(datosFiltrados);

// Acceder a resultados
decimal media = estadisticas.MediaGastos;
foreach(var top3 in estadisticas.Top3Gastos)
{
    string tipo = top3.Tipus;
    decimal total = top3.TotalGasto;
}
```

## Archivos Modificados

- ✅ `Despesa.cs` - Añadido método `CalcularEstadisticas`
- ✅ `ReportForm.cs` - Actualizado para cargar estadísticas
- ✅ `ReportDespeses.rdlc` - Añadidos DataSets y elementos visuales
- ✅ `EstadisticaResultado.cs` - Nuevo archivo con clases
- ✅ `estadi.cob` - Nuevo programa COBOL (opcional)

## Mejoras Futuras

- Cacheo de resultados para grandes volúmenes de datos
- Gráfico adicional para el top 3
- Exportación de estadísticas a PDF/Excel
- Comparativa con períodos anteriores
