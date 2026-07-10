CREATE DATABASE SmallChange;
GO
USE SmallChange;
GO

CREATE TABLE Clientes (
    id INT IDENTITY(1,1) PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    email VARCHAR(100) UNIQUE NOT NULL,
    pass_hash VARCHAR(255) NOT NULL,
    calificacion_vendedor DECIMAL(3,2) DEFAULT 0.00,
    fecha_registro DATETIME2 DEFAULT GETDATE()
);

CREATE TABLE Ofertas (
    id INT IDENTITY(1,1) PRIMARY KEY,
    cliente_id INT,
    moneda_a_enviar VARCHAR(10) NOT NULL,
    moneda_a_recibir VARCHAR(10) NOT NULL,
    cantidad DECIMAL(18,2) NOT NULL,
    tipo_cambio DECIMAL(10,4) NOT NULL,
    estado BIT DEFAULT 1,
    fecha_creacion DATETIME2 DEFAULT GETDATE(),
    CONSTRAINT FK_Ofertas_Clientes FOREIGN KEY (cliente_id) 
        REFERENCES Clientes(id) ON DELETE CASCADE
);

CREATE TABLE AuditoriasTransacciones (
	id INT IDENTITY(1,1) PRIMARY KEY,
	transaccion_id INT NOT NULL,
	usuario_id INT NOT NULL,
	accion VARCHAR(20) NOT NULL,              -- 'CREAR', 'ACTUALIZAR_ESTADO', 'ELIMINAR'
	estado_anterior VARCHAR(50) NULL,
	estado_nuevo VARCHAR(50) NOT NULL,
	fecha_accion DATETIME2 DEFAULT GETUTCDATE(),
	CONSTRAINT FK_AuditoriasTransacciones_Transaccion 
		FOREIGN KEY (transaccion_id) REFERENCES Transacciones(id),
	CONSTRAINT FK_AuditoriasTransacciones_Clientes 
		FOREIGN KEY (usuario_id) REFERENCES dbo.Clientes(id)
);


CREATE NONCLUSTERED INDEX IX_AuditoriasTransacciones_TransaccionId 
	ON dbo.AuditoriasTransacciones(transaccion_id);

CREATE NONCLUSTERED INDEX IX_AuditoriasTransacciones_UsuarioId 
	ON dbo.AuditoriasTransacciones(usuario_id);

CREATE NONCLUSTERED INDEX IX_AuditoriasTransacciones_FechaAccion 
	ON dbo.AuditoriasTransacciones(fecha_accion DESC);
