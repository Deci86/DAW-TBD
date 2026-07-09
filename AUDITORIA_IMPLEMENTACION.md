## Endpoints de Auditoría

### 1. Obtener Historial de una Transacción Específica
```http
GET /api/transacciones/{id}/auditoria
Authorization: Bearer {token}
```

**Descripción:** Obtiene el historial completo de cambios de una transacción específica.


```json
[
  {
	"id": 1,
	"transaccionId": 5,
	"usuarioId": 2,
	"accion": "CREAR",
	"estadoAnterior": null,
	"estadoNuevo": "pendiente",
	"fechaAccion": "2024-01-15T10:30:00Z",
	"usuario": {
	  "id": 2,
	  "nombre": "Juan Pérez",
	  "email": "juan@example.com"
	}
  },
  {
	"id": 2,
	"transaccionId": 5,
	"usuarioId": 3,
	"accion": "ACTUALIZAR_ESTADO",
	"estadoAnterior": "pendiente",
	"estadoNuevo": "completada",
	"fechaAccion": "2024-01-15T12:45:00Z",
	"usuario": {
	  "id": 3,
	  "nombre": "María González",
	  "email": "maria@example.com"
	}
  }
]
```

---

### 2. Obtener Mi Historial de Auditorías
```http
GET /api/transacciones/auditoria/mi-historial
Authorization: Bearer {token}
```

**Descripción:** Obtiene todas las auditorías (acciones) realizadas por el usuario autenticado.


```json
[
  {
	"id": 1,
	"transaccionId": 5,
	"usuarioId": 2,
	"accion": "CREAR",
	"estadoAnterior": null,
	"estadoNuevo": "pendiente",
	"fechaAccion": "2024-01-15T10:30:00Z",
	"usuario": {
	  "id": 2,
	  "nombre": "Juan Pérez",
	  "email": "juan@example.com"
	}
  },
  {
	"id": 3,
	"transaccionId": 7,
	"usuarioId": 2,
	"accion": "ACTUALIZAR_ESTADO",
	"estadoAnterior": "pendiente",
	"estadoNuevo": "cancelada",
	"fechaAccion": "2024-01-15T14:20:00Z",
	"usuario": {
	  "id": 2,
	  "nombre": "Juan Pérez",
	  "email": "juan@example.com"
	}
  }
]
```

---

## Flujo de Auditoría

### Crear Transacción
```
-POST /api/transacciones

-TransaccionesService.AddAsync(createDto, usuarioId)

-Se crea la transacción en BD

-AuditoriaService.RegistrarCreacionAsync()

-Se inserta en AuditoriaTransacciones:
  accion = "CREAR"
  estado_anterior = null
  estado_nuevo = "pendiente"
```

### Actualizar Estado de Transacción
```
-PUT /api/transacciones/{id}

-TransaccionesService.UpdateAsync(id, updateDto, usuarioId)

-Se actualiza estado en BD

-AuditoriaService.RegistrarCambioEstadoAsync()

-Se inserta en AuditoriaTransacciones:
  accion = "ACTUALIZAR_ESTADO"
  estado_anterior = estado previo
  estado_nuevo = nuevo estado
```

### Eliminar Transacción
```
-DELETE /api/transacciones/{id}

-TransaccionesService.DeleteAsync(id, usuarioId)

-Se elimina la transacción de BD

-AuditoriaService.RegistrarEliminacionAsync()

-Se inserta en AuditoriaTransacciones:
  accion = "ELIMINAR"
  estado_anterior = estado previo
  estado_nuevo = "eliminada"
```

---

## Seguridad y Permisos

### Para el endpoint de historial de una transacción:
- Solo usuarios involucrados en la transacción (comprador o vendedor) pueden ver su auditoría
- Se valida: `transaccion.ClienteCompradorId == usuarioId || transaccion.Oferta.cliente_id == usuarioId`

### Para el endpoint de mi historial:
- Cada usuario solo ve sus propias acciones (auditorías donde `usuario_id == usuarioId`)

---

## Acciones Registradas

Las siguientes acciones se registran automáticamente:

| Acción | Cuándo | Estado Anterior | Estado Nuevo |
|--------|--------|-----------------|--------------|
| **CREAR** | Al crear una transacción | `null` | `"pendiente"` |
| **ACTUALIZAR_ESTADO** | Al cambiar estado | Estado previo | Nuevo estado |
| **ELIMINAR** | Al eliminar transacción | Estado previo | `"eliminada"` |

---

## Notas Importantes

1. **usuarioId**: Se extrae automáticamente del token JWT (`ClaimTypes.NameIdentifier`)
2. **Timestamps**: Se registran en UTC con precisión de milisegundos (`DATETIME2`)
3. **Ordenamiento**: El historial se devuelve ordenado por fecha descendente (más reciente primero)
4. **Cascada de restricciones**: Las claves foráneas están configuradas con `ON DELETE RESTRICT` para proteger la integridad

---


