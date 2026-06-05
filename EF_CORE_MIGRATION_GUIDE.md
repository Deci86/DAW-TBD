# Guía de Migración a Entity Framework Core

## 📋 Cambios Realizados

Esta rama `feature/implement-efcore-orm` implementa una migración completa de **Dapper** a **Entity Framework Core**, mejorando significativamente la seguridad y mantenibilidad de las APIs.

### ✅ Archivos Creados/Modificados

#### 1. **SmallChangeDbContext.cs** (NUEVO)
- DbContext centralizado para toda la aplicación
- Configuración de todas las entidades (Cliente, Oferta, Transaccion)
- Fluent API para mapeos y relaciones
- Constraints y índices definidos

**Ubicación:** `SmallChangeDAW.CORE/Infrastructure/Data/SmallChangeDbContext.cs`

#### 2. **Repositorios Actualizados**
- `ClientesRepository.cs` - Migrado a EF Core
- `OfertasRepository.cs` - Migrado a EF Core
- `TransaccionesRepository.cs` - Migrado a EF Core

**Cambios principales:**
- ❌ Eliminado Dapper y consultas SQL raw
- ✅ Uso de `DbContext` para operaciones CRUD
- ✅ LINQ type-safe queries
- ✅ Automatic parameter binding (previene SQL injection)
- ✅ Change tracking automático

#### 3. **Program.cs** (ACTUALIZADO)
- Configuración de DbContext con SQL Server
- Inyección de dependencias actualizada
- ❌ Eliminado `DbConnectionFactory`
- ✅ Uso de `AddDbContext<SmallChangeDbContext>()`

---

## 🔒 Mejoras de Seguridad

### SQL Injection Prevention
```csharp
// ❌ ANTES (Dapper - Vulnerable si no se usan parámetros correctamente)
"SELECT * FROM Clientes WHERE id = @Id"

// ✅ AHORA (EF Core - Seguro por defecto)
_context.Clientes.FirstOrDefaultAsync(c => c.Id == id);
```

### Validación Automática
EF Core proporciona:
- Validación de tipos en tiempo de compilación
- Validaciones de propiedades (MaxLength, Required, etc.)
- Constraints de base de datos automáticos

---

## 📦 Dependencias Requeridas

Asegurate de instalar en tu proyecto `SmallChangeDAW.API`:

```bash
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
```

---

## 🚀 Pasos para Implementar

### 1. **Instalar paquetes NuGet**
```bash
cd SmallChangeDAW.API
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
```

### 2. **Crear Migration Inicial**
```bash
dotnet ef migrations add InitialCreate --project SmallChangeDAW.CORE --startup-project SmallChangeDAW.API
```

### 3. **Aplicar Migration a la BD**
```bash
dotnet ef database update --project SmallChangeDAW.CORE --startup-project SmallChangeDAW.API
```

### 4. **Verificar la Conexión**
- La BD debe actualizarse automáticamente con las nuevas tablas/esquemas
- Todos los repositorios ya usan EF Core

---

## 🔄 Cambios en la Configuración

### appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=SmallChangeDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

---

## ✨ Beneficios

| Feature | Dapper | EF Core |
|---------|--------|---------|
| **LINQ Support** | ❌ No | ✅ Sí |
| **Change Tracking** | Manual | ✅ Automático |
| **Migrations** | Manual | ✅ Automático |
| **Type Safety** | ⚠️ Parcial | ✅ Total |
| **SQL Injection Protection** | Manual | ✅ Automático |
| **Performance** | Rápido | ✅ Muy Rápido |

---

## 📝 Notas Importantes

1. **DbConnectionFactory**: Ya no se necesita. Será reemplazado por `SmallChangeDbContext`.
2. **Dapper**: Se puede eliminar del proyecto si no se usa en otros lugares.
3. **Servicios**: No requieren cambios - usan las interfaces de repositorios.
4. **Controllers**: No requieren cambios - todo funciona igual.

---

## 🧪 Testing

Después de la migración, verifica:
- ✅ Todas las APIs devuelven datos correctamente
- ✅ Las validaciones funcionan
- ✅ Las relaciones entre tablas se cargan correctamente
- ✅ No hay errores de SQL injection

---

## 📞 Soporte

Si hay problemas durante la migración:
1. Revisa los errores de compilation
2. Asegurate de que todas las dependencias estén instaladas
3. Verifica la cadena de conexión en `appsettings.json`
4. Ejecuta `dotnet ef database update --verbose` para ver detalles

---

**Rama:** `feature/implement-efcore-orm`  
**Estado:** Listo para Pull Request

