-- Ejecuta esto en SQL Server Management Studio

USE SmallChange; -- Cambia esto por tu BD real

-- Verificar si la tabla ya existe
IF OBJECT_ID(N'dbo.AuditoriasTransacciones', N'U') IS NOT NULL
	DROP TABLE dbo.AuditoriasTransacciones;

-- Crear tabla
CREATE TABLE dbo.AuditoriasTransacciones (
	id INT IDENTITY(1,1) PRIMARY KEY,
	transaccion_id INT NOT NULL,
	usuario_id INT NOT NULL,
	accion VARCHAR(20) NOT NULL,              -- 'CREAR', 'ACTUALIZAR_ESTADO', 'ELIMINAR'
	estado_anterior VARCHAR(50) NULL,
	estado_nuevo VARCHAR(50) NOT NULL,
	fecha_accion DATETIME2 DEFAULT GETUTCDATE(),

	-- Claves foráneas
	CONSTRAINT FK_AuditoriasTransacciones_Transaccion 
		FOREIGN KEY (transaccion_id) REFERENCES Transacciones(id),

	CONSTRAINT FK_AuditoriasTransacciones_Clientes 
		FOREIGN KEY (usuario_id) REFERENCES dbo.Clientes(id)
);

-- Crear índices para optimizar consultas
CREATE NONCLUSTERED INDEX IX_AuditoriasTransacciones_TransaccionId 
	ON dbo.AuditoriasTransacciones(transaccion_id);

CREATE NONCLUSTERED INDEX IX_AuditoriasTransacciones_UsuarioId 
	ON dbo.AuditoriasTransacciones(usuario_id);

CREATE NONCLUSTERED INDEX IX_AuditoriasTransacciones_FechaAccion 
	ON dbo.AuditoriasTransacciones(fecha_accion DESC);

-- Verificar que la tabla se creó correctamente
SELECT 
	COLUMN_NAME, 
	DATA_TYPE, 
	IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'AuditoriasTransacciones'
ORDER BY ORDINAL_POSITION;

PRINT 'Tabla AuditoriasTransacciones creada correctamente.';
