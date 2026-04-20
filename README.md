# ProjecteCobolDavid

Aplicació de gestió de despeses que integra C# (.NET 10) amb COBOL per a l'emmagatzematge i visualització de dades en format de fitxer `.DAT`.

## 📋 Funcions Principals

### 1. Entrada de Dades
- Formulari amb 4 camps obligatoris:
  - **Nom de la despesa** (màxim 30 caracteres)
  - **Cost** (número decimal amb 2 decimals)
  - **Data** (selector de data)
  - **Tipus de despesa** (ComboBox amb opcions: Compra, Despeses de la llar, Alquiler, Oci, Hipoteca)
- Validació obligatòria de tots els camps antes d'enviar

### 2. Emmagatzematge
- Integració amb programa COBOL (`guardar.exe`) que escriu les dades en format de registres de longitud fixa
- Fitxer de base de dades simple: `DESPESES.DAT`
- Format de registre: 68 bytes (30 + 8 + 10 + 20)

### 3. Visualització
- **DataGridView** dinàmic amb:
  - Suport d'ordenació per clic en les capçaleres de columna
  - Formateo automàtic de la columna Cost (2 decimals)
  - Alternança entre ascendent/descendent

### 4. Gestió de Dades
- Botó **Actualitzar**: Recarrega les dades del `.DAT`
- Botó **Borrar .DAT**: Borra tot el contingut amb confirmació de l'usuari
- Validació i manejo de errors en la lectura

### 5. Generació d'Informes
- Informe RDLC (`ReportDespeses.rdlc`) amb visualizador (`rpvDespeses`)
- Mostra les despeses en format de informe professionalitzat
- Suport per a gràfics i totals (personalitzables en el `.rdlc`)

## 🛠️ Tecnologies Emprades

| Tecnologia | Versió | Propòsit |
|-----------|--------|---------|
| **.NET** | 10.0 | Framework base |
| **C#** | Últim | Llenguatge de programació principal |
| **WinForms** | .NET 10 | Interfície d'usuari |
| **COBOL** | GnuCOBOL / Micro Focus | Processament i emmagatzematge de dades |
| **RDLC** | SQL Server 2019+ | Definició d'informes |
| **ReportViewer** | .NET WinForms | Visualizador d'informes |
| **System.Diagnostics** | Nativa | Execució de programes COBOL |
| **System.IO** | Nativa | Lectura/escriptura de fitxers |
| **System.Reflection** | Nativa | Ordenació dinàmica |
| **LINQ** | .NET 10 | Consultes i transformació de dades |

## 📊 Flux de Dades

```
┌─────────────────────────────────────────────────────────────────┐
│                    INTERFÍCIE D'USUARI (WinForms)                 │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────────────┐   │
│  │ txtNom       │  │ numCost      │  │ cmbTipus             │   │
│  │ dtpData      │  │ btnGuardar   │  │ btnActualitzar       │   │
│  │ btnMostrarIn.│  │ btnBorrarDat │  │ btnBorrarDat         │   │
│  └──────────────┘  └──────────────┘  └──────────────────────┘   │
│                                                                   │
│  ┌──────────────────────────────────────────────────────────┐    │
│  │ DataGridView: Visualització de totes les despeses        │    │
│  │ (ordenable per clic en capçaleres)                       │    │
│  └──────────────────────────────────────────────────────────┘    │
└──────────────────────────────────┬──────────────────────────────┘
                                   │
                    ┌──────────────┴──────────────┐
                    │                             │
         ┌──────────▼──────────┐      ┌───────────▼─────────────┐
         │ EnviarACobol()      │      │ CarregarDades()         │
         │ (Validació + envío) │      │ (Lectura del .DAT)      │
         └──────────┬──────────┘      └───────────┬─────────────┘
                    │                             │
         ┌──────────▼────────────────────────────┴──────────┐
         │    Programa COBOL (guardar.exe)                  │
         │ ┌─────────────────────────────────────────────┐  │
         │ │ IDENTIFICATION: ECONOPARSE-SAVE             │  │
         │ │ INPUT: 4 arguments (Nom, Cost, Data, Tipus) │  │
         │ │ OUTPUT: DESPESES.DAT (registres de 68 bytes)│  │
         │ └─────────────────────────────────────────────┘  │
         └──────────────┬───────────────────────────────────┘
                        │
         ┌──────────────▼────────────────────┐
         │    DESPESES.DAT                   │
         │  (Format fix de registres)        │
         │  ┌─────────────────────────────┐  │
         │  │ Bytes 0-29:  Nom (30 chars) │  │
         │  │ Bytes 30-37: Cost (8 dígits)│  │
         │  │ Bytes 38-47: Data (10 chars)│  │
         │  │ Bytes 48-67: Tipus (20 ch)  │  │
         │  └─────────────────────────────┘  │
         └──────────────┬────────────────────┘
                        │
         ┌──────────────▼──────────────────-──┐
         │  Lectura i processament            │
         │  - Clase Despesa.CarregarDades()   │
         │  - Parsing de registres            │
         │  - Conversió de tipus              │
         └──────────────┬─────────────────-───┘
                        │
         ┌──────────────▼────────────────────┐
         │  Visualització i Informes         │
         │  ┌────────────────────────────┐   │
         │  │ DataGridView               │   │
         │  │ (ordenació, formateo)      │   │
         │  └────────────────────────────┘   │
         │  ┌────────────────────────────┐   │
         │  │ ReportViewer (RDLC)        │   │
         │  │ (informes personalitzats)  │   │
         │  └────────────────────────────┘   │
         └───────────────────────────────────┘
```

## 📝 Format del Fitxer DESPESES.DAT

Cada registre ocupa exactament **68 bytes**:

| Camp | Bytes | Tipus | Format | Exemple |
|------|-------|-------|--------|---------|
| Nom | 0-29 | Text | PIC X(30) | `Sopar                        ` |
| Cost | 30-37 | Numèric | PIC 9(06)V99 | `00002537` (25.37) |
| Data | 38-47 | Text | PIC X(10) | `2024-05-20` |
| Tipus | 48-67 | Text | PIC X(20) | `Oci                 ` |

## 🔄 Procés de Validació

```
Entrada d'usuari
    │
    ▼
ValidateInputs()
    │
    ├─ Nom no buit?
    ├─ Cost > 0?
    └─ Tipus seleccionat?
    │
    ├─ NO ──► MessageBox amb camps faltants ──► STOP
    │
    └─ SÍ
        │
        ▼
    Crear objecte Despesa
        │
        ▼
    Despesa.EnviarACobol()
        │
        ▼
    Executar guardar.exe
        │
        ▼
    DESPESES.DAT actualitzat
        │
        ▼
    Recarregar DataGridView
        │
        ▼
    MessageBox d'èxit
```

## 💾 Estructura de Classes

### Despesa.cs
```csharp
public class Despesa
{
    public string Nom { get; set; }          // Màx 30 caracteres
    public decimal Cost { get; set; }        // 2 decimals (format moneda)
    public DateTime Data { get; set; }       // Format yyyy-MM-dd
    public string Tipus { get; set; }        // Màx 20 caracteres

    public static void EnviarACobol(Despesa d)      // Envia al COBOL
    public static List<Despesa> CarregarDades()     // Llegeix del .DAT
    public static void BorrarDat()                   // Borra el .DAT
}
```

### Form1.cs
- **Gestió de Controls**: TextBox, NumericUpDown, DateTimePicker, ComboBox, DataGridView
- **Ordenació**: dgvDespeses_ColumnHeaderMouseClick
- **Validació**: ValidateInputs
- **Informe**: btnMostrarInforme_Click (obreix ReportForm)
- **Formateo**: FormatCostColumn

### ReportForm.cs
- **Visualizador**: ReportViewer
- **Càrrega**: LoadReport(List<Despesa>)
- **Integració RDLC**: ReportDespeses.rdlc

## 🚀 Com Executar

### 1. Requisits Previs
- .NET 10.0 SDK instal·lat
- Visual Studio Community 2026 o superior
- GnuCOBOL o Micro Focus COBOL (per compilar guardar.cbl)

### 2. Compilar el Projecte
```powershell
cd C:\Users\DavidCOBOL\Cobol\ProjecteFinal\ProjecteCobolDavid
dotnet build
```

### 3. Compilar el Programa COBOL
```powershell
# Amb GnuCOBOL
cobc -x -o guardar.exe guardar.cbl

# O amb Micro Focus
cob -x guardar.cbl
```

### 4. Executar l'Aplicació
```powershell
dotnet run
```

## 📌 Notes Importants

- **Consistència de la moneda**: El sistema usa `CultureInfo.InvariantCulture` per assegurar consistència en la lectura/escriptura de decimals.
- **Longitud fixa**: Tots els registres al `.DAT` usen longitud exacta (68 bytes) per compatibilitat amb COBOL.
- **Validació doble**: Els camps es validen tant en C# (obligatoris) com a COBOL (format).
- **Informes**: El `.rdlc` usa `DataSet1` que ha d'estar configurat amb els 4 camps de Despesa.

## 📞 Contacte

- **Autor**: David Romero
- **Repositori**: https://github.com/DavidRomeroSanMillan/ProjecteCobolDavid

---

**Última actualització**: 2026