CREATE DATABASE SmallChange;
GO
USE SmallChange;
GO

CREATE TABLE Clientes (
    id INT IDENTITY(1,1) PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    email VARCHAR(100) UNIQUE NOT NULL,
    pass_hash VARCHAR(255) NOT NULL,
    promedio_calificacion_comprador DECIMAL(3,2) DEFAULT 0.00,
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

CREATE TABLE Transacciones (
    id INT IDENTITY(1,1) PRIMARY KEY,
    oferta_id INT,
    cliente_comprador_id INT,
    fecha_transaccion DATETIME2 DEFAULT GETDATE(),
    estado NVARCHAR(20) DEFAULT 'pendiente',
    CONSTRAINT CK_Estado_Transaccion CHECK (estado IN ('pendiente', 'completada', 'cancelada')),
    CONSTRAINT FK_Transacciones_Ofertas FOREIGN KEY (oferta_id) REFERENCES Ofertas(id),
    CONSTRAINT FK_Transacciones_Clientes FOREIGN KEY (cliente_comprador_id) REFERENCES Clientes(id)
);
