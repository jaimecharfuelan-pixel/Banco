-- ============================================================
-- SCRIPT COMPLETO DE BASE DE DATOS - VERSIÓN ACTUAL
-- Banco - Sistema de Gestión Bancaria
-- ============================================================
-- Este script contiene la estructura completa de la base de datos:
-- - Roles
-- - Tablas
-- - Triggers
-- - Packages (procedimientos almacenados)
-- - Grants (permisos)
--
-- Fecha de actualización: 16 de Noviembre de 2025
-- Versión: Actual (con todas las correcciones aplicadas)
--
-- NOTA: Este script NO incluye el prefijo BANCO. en los nombres
-- de tablas. Todas las tablas se crean sin prefijo.
-- ============================================================

CREATE ROLE ROL_ADMIN;
CREATE ROLE ROL_CLIENTE;


CREATE TABLE ABONO_EXTRAORDINARIO (
    ID_ABONO NUMBER(12),
    ID_PRESTAMO NUMBER(12),
    ID_TRANSACCION NUMBER(12),
    MONTO_ABONO NUMBER(15,2),
    FECHA_ABONO TIMESTAMP,
    TIPO_ABONO VARCHAR2(20),

    CONSTRAINT PK_ABONO PRIMARY KEY (ID_ABONO),
    CONSTRAINT CK_ABONO_MONTO CHECK (MONTO_ABONO > 0),
    CONSTRAINT CK_ABONO_TIPO CHECK (TIPO_ABONO IN ('reduccion de plazo', 'reduccion de cuota')),
    CONSTRAINT UQ_ABONO_TRX UNIQUE (ID_TRANSACCION),
    CONSTRAINT FK_ABONO_PRESTAMO FOREIGN KEY (ID_PRESTAMO) REFERENCES PRESTAMO(ID_PRESTAMO),
    CONSTRAINT FK_ABONO_TRX FOREIGN KEY (ID_TRANSACCION) REFERENCES TRANSACCION(ID_TRANSACCION)
);

CREATE TABLE ADMINISTRADOR (
    ID_ADMINISTRADOR NUMBER(12),
    FECHA_ASIGNACION_ADMINISTRADOR TIMESTAMP,
    EMAIL_ADMINISTRADOR VARCHAR2(120),
    CONTRASENA_ADMINISTRADOR VARCHAR2(50),

    CONSTRAINT PK_ADMINISTRADOR PRIMARY KEY (ID_ADMINISTRADOR)
);

CREATE TABLE CAJERO (
    ID_CAJERO NUMBER(12),
    ID_SUCURSAL NUMBER(12),
    ID_ADMINISTRADOR NUMBER(12),
    DINERO_DISPONIBLE_CAJERO NUMBER(15,2),
    ESTADO_CAJERO VARCHAR2(20),
    FECHA_ULTIMA_RECARGA_CAJERO TIMESTAMP,

    CONSTRAINT PK_CAJERO PRIMARY KEY (ID_CAJERO),
    CONSTRAINT CK_CAJERO_DINERO CHECK (DINERO_DISPONIBLE_CAJERO >= 0),
    CONSTRAINT CK_CAJERO_ESTADO CHECK (ESTADO_CAJERO IN ('activo','inactivo')),
    CONSTRAINT FK_CAJERO_SUCURSAL FOREIGN KEY (ID_SUCURSAL) REFERENCES SUCURSAL(ID_SUCURSAL),
    CONSTRAINT FK_CAJERO_ADMIN FOREIGN KEY (ID_ADMINISTRADOR) REFERENCES ADMINISTRADOR(ID_ADMINISTRADOR)
);

CREATE OR REPLACE TRIGGER TRG_AUTO_ID_CAJERO
BEFORE INSERT ON CAJERO
FOR EACH ROW
DECLARE 
    V_ID NUMBER;
BEGIN
    IF :NEW.ID_CAJERO IS NULL THEN
        SELECT NVL(MAX(ID_CAJERO),0) + 1 INTO V_ID FROM CAJERO;
        :NEW.ID_CAJERO := V_ID;
    END IF;
END;
/

CREATE TABLE CLIENTE (
    NOMBRE_CLIENTE VARCHAR2(120),
    EMAIL_CLIENTE VARCHAR2(120),
    TELEFONO_CLIENTE VARCHAR2(30),
    ESTADO_CLIENTE VARCHAR2(10),
    FECHA_REGISTRO_CLIENTE TIMESTAMP,
    CEDULA_CLIENTE NUMBER(12),
    ID_CLIENTE VARCHAR2(10),

    CONSTRAINT PK_CLIENTE PRIMARY KEY (ID_CLIENTE),
    CONSTRAINT CK_CLIENTE_ESTADO CHECK (ESTADO_CLIENTE IN ('ACTIVO','INACTIVO')),
    CONSTRAINT UQ_CLIENTE_EMAIL UNIQUE (EMAIL_CLIENTE),
    CONSTRAINT UQ_CLIENTE_CEDULA UNIQUE (CEDULA_CLIENTE)
);

CREATE TABLE CUENTA (
    ID_CUENTA NUMBER(12),
    ID_CLIENTE VARCHAR2(10) NOT NULL,
    ID_ADMINISTRADOR NUMBER(12) NOT NULL,
    SALDO_CUENTA NUMBER(15,2),
    FECHA_CREACION_CUENTA TIMESTAMP,
    ESTADO_CUENTA VARCHAR2(10),
    FECHA_ULTIMA_TRANSACCION_CUENTA TIMESTAMP,
    CONTRASENA_CUENTA VARCHAR2(30),

    CONSTRAINT PK_CUENTA PRIMARY KEY (ID_CUENTA),
    CONSTRAINT CK_CUENTA_SALDO CHECK (SALDO_CUENTA >= 0),
    CONSTRAINT CK_CUENTA_ESTADO CHECK (ESTADO_CUENTA IN ('activa','inactiva')),
    CONSTRAINT FK_CUENTA_CLIENTE FOREIGN KEY (ID_CLIENTE) REFERENCES CLIENTE(ID_CLIENTE),
    CONSTRAINT FK_CUENTA_ADMIN FOREIGN KEY (ID_ADMINISTRADOR) REFERENCES ADMINISTRADOR(ID_ADMINISTRADOR)
);

CREATE TABLE CUOTA (
    ID_CUOTA NUMBER(12),
    ID_PRESTAMO NUMBER(12),
    NUMERO_CUOTA NUMBER(6),
    MONTO_DE_CUOTA NUMBER(15,2),
    CAPITAL_CUOTA NUMBER(15,2),
    FECHA_DE_VENCIMIENTO_CUOTA TIMESTAMP,
    FECHA_DE_PAGO_CUOTA TIMESTAMP,
    ESTADO_CUOTA VARCHAR2(10),

    CONSTRAINT PK_CUOTA PRIMARY KEY (ID_CUOTA),
    CONSTRAINT CK_CUOTA_NUM CHECK (NUMERO_CUOTA > 0),
    CONSTRAINT CK_CUOTA_MONTOS CHECK (MONTO_DE_CUOTA >= 0 AND CAPITAL_CUOTA >= 0),
    CONSTRAINT CK_CUOTA_MONTO_CAPITAL CHECK (MONTO_DE_CUOTA >= CAPITAL_CUOTA AND CAPITAL_CUOTA >= 0),
    CONSTRAINT CK_CUOTA_ESTADO CHECK (ESTADO_CUOTA IN ('pendiente','pagada','mora')),
    CONSTRAINT UQ_CUOTA_PREST UNIQUE (ID_PRESTAMO, NUMERO_CUOTA),
    CONSTRAINT FK_CUOTA_PRESTAMO FOREIGN KEY (ID_PRESTAMO) REFERENCES PRESTAMO(ID_PRESTAMO)
);

CREATE TABLE PRESTAMO (
    ID_PRESTAMO NUMBER(12),
    ID_SUCURSAL NUMBER(12),
    MONTO_PRESTAMO NUMBER(15,2),
    TASA_DE_INTERES_PRESTAMO NUMBER(5,2),
    PLAZO_PRESTAMO NUMBER(6),
    FECHA_INICIO_PRESTAMO TIMESTAMP,
    FECHA_VENCIMIENTO_PRESTAMO TIMESTAMP,
    ESTADO_PRESTAMO VARCHAR2(12),
    FECHA_ULTIMO_PAGO_PRESTAMO TIMESTAMP,
    SALDO_PRESTAMO NUMBER(15,2),
    ID_CLIENTE VARCHAR2(10),

    CONSTRAINT PK_PRESTAMO PRIMARY KEY (ID_PRESTAMO),

    CONSTRAINT CK_PRESTAMO_MONTO CHECK (MONTO_PRESTAMO > 0),
    CONSTRAINT CK_PRESTAMO_TASA CHECK (TASA_DE_INTERES_PRESTAMO BETWEEN 0 AND 15),
    CONSTRAINT CK_PRESTAMO_SALDO CHECK (SALDO_PRESTAMO >= 0),
    CONSTRAINT CK_PRESTAMO_ESTADO CHECK (ESTADO_PRESTAMO IN ('solicitado','activo','mora','finalizado','rechazado')),
    CONSTRAINT CK_PRESTAMO_PLAZO CHECK (
        (ESTADO_PRESTAMO = 'solicitado' AND PLAZO_PRESTAMO = 0) OR
        (ESTADO_PRESTAMO <> 'solicitado' AND PLAZO_PRESTAMO > 0)
    ),

    CONSTRAINT FK_PRESTAMO_SUCURSAL FOREIGN KEY (ID_SUCURSAL) REFERENCES SUCURSAL(ID_SUCURSAL),
    CONSTRAINT FK_PRESTAMO_CLIENTE FOREIGN KEY (ID_CLIENTE) REFERENCES CLIENTE(ID_CLIENTE)
);

CREATE TABLE SUCURSAL (
    ID_SUCURSAL NUMBER(12),
    NOMBRE_SUCURSAL VARCHAR2(120),
    DIRECCION_SUCURSAL VARCHAR2(200),
    TELEFONO_SUCURSAL VARCHAR2(30),
    ESTADO_SUCURSAL VARCHAR2(10),
    ID_ADMINISTRADOR NUMBER(12),

    CONSTRAINT PK_SUCURSAL PRIMARY KEY (ID_SUCURSAL),
    CONSTRAINT CK_SUCURSAL_ESTADO CHECK (ESTADO_SUCURSAL IN ('abierta','cerrada')),
    CONSTRAINT FK_SUCURSAL_ADMIN FOREIGN KEY (ID_ADMINISTRADOR) REFERENCES ADMINISTRADOR(ID_ADMINISTRADOR)
);

CREATE TABLE TARJETA (
    ID_TARJETA NUMBER(12),
    ID_CUENTA NUMBER(12),
    NUMERO_TARJETA VARCHAR2(25),
    CVV_TARJETA VARCHAR2(3),
    FECHA_EMISION_TARJETA TIMESTAMP,
    FECHA_VENCIMIENTO_TARJETA TIMESTAMP,
    ESTADO_TARJETA VARCHAR2(10),

    CONSTRAINT PK_TARJETA PRIMARY KEY (ID_TARJETA),
    CONSTRAINT CK_TARJETA_ESTADO CHECK (
        ESTADO_TARJETA IN ('activa','inactiva','bloqueada','suspendida')
    ),
    CONSTRAINT UQ_TARJETA_NUMERO UNIQUE (NUMERO_TARJETA),
    CONSTRAINT FK_TARJETA_CUENTA FOREIGN KEY (ID_CUENTA) REFERENCES CUENTA(ID_CUENTA)
);


CREATE TABLE TARJETA_CREDITO (
    ID_TARJETA NUMBER(12),
    TASA_INTERES_CREDITO NUMBER(5,2),
    LIMITE_CREDITO NUMBER(15,2),
    CUOTA_MANEJO_CREDITO NUMBER(15,2),
    FECHA_PAGO_CREDITO TIMESTAMP,
    FECHA_CORTE_CREDITO NUMBER(2),

    CONSTRAINT PK_TARJETA_CREDITO PRIMARY KEY (ID_TARJETA),
    CONSTRAINT CK_CREDITO_TASA CHECK (TASA_INTERES_CREDITO >= 0),
    CONSTRAINT CK_CREDITO_LIMITE CHECK (LIMITE_CREDITO >= 0),
    CONSTRAINT CK_CREDITO_CUOTA CHECK (CUOTA_MANEJO_CREDITO >= 0),
    CONSTRAINT CK_CREDITO_CORTE CHECK (FECHA_CORTE_CREDITO BETWEEN 1 AND 31),
    CONSTRAINT FK_CREDITO_TARJETA FOREIGN KEY (ID_TARJETA) REFERENCES TARJETA(ID_TARJETA)
);


CREATE TABLE TARJETA_DEBITO (
    ID_TARJETA NUMBER(12),
    LIMITE_RETIRO_DEBITO NUMBER(15,2),
    SALDO_ACTUAL_DEBITO NUMBER(15,2),

    CONSTRAINT PK_TARJETA_DEBITO PRIMARY KEY (ID_TARJETA),
    CONSTRAINT CK_DEBITO_LIMITE CHECK (LIMITE_RETIRO_DEBITO >= 0),
    CONSTRAINT CK_DEBITO_SALDO CHECK (SALDO_ACTUAL_DEBITO >= 0),
    CONSTRAINT FK_DEBITO_TARJETA FOREIGN KEY (ID_TARJETA) REFERENCES TARJETA(ID_TARJETA)
);


CREATE TABLE TRANSACCION (
    ID_TRANSACCION NUMBER(12),
    ID_CAJERO NUMBER(12),
    ID_TARJETA NUMBER(12),
    ID_CUENTA_ORIGEN NUMBER(12),
    ID_CUENTA_DESTINO NUMBER(12),
    FECHA_TRANSACCION TIMESTAMP,
    TIPO_TRANSACCION VARCHAR2(20),
    MONTO_TRANSACCION NUMBER(15,2),
    ESTADO_TRANSACCION VARCHAR2(12),
    DESCRIPCION_TRANSACCION VARCHAR2(45),

    CONSTRAINT PK_TRANSACCION PRIMARY KEY (ID_TRANSACCION),
    CONSTRAINT CK_TRX_TIPO CHECK (TIPO_TRANSACCION IN ('cuenta','cajero')),
    CONSTRAINT CK_TRX_MONTO CHECK (MONTO_TRANSACCION > 0),
    CONSTRAINT CK_TRX_ESTADO CHECK (
        ESTADO_TRANSACCION IN ('completada','pendiente','fallida','cancelada')
    ),

    CONSTRAINT FK_TRX_CAJERO FOREIGN KEY (ID_CAJERO) REFERENCES CAJERO(ID_CAJERO),
    CONSTRAINT FK_TRX_TARJETA FOREIGN KEY (ID_TARJETA) REFERENCES TARJETA(ID_TARJETA),
    CONSTRAINT FK_TRX_CTA_ORIGEN FOREIGN KEY (ID_CUENTA_ORIGEN) REFERENCES CUENTA(ID_CUENTA),
    CONSTRAINT FK_TRX_CTA_DEST FOREIGN KEY (ID_CUENTA_DESTINO) REFERENCES CUENTA(ID_CUENTA)
);


GRANT SELECT, INSERT, UPDATE, DELETE ON ABONO_EXTRAORDINARIO TO ROL_ADMIN;
GRANT SELECT ON ABONO_EXTRAORDINARIO TO ROL_CLIENTE;

GRANT SELECT, INSERT, UPDATE, DELETE ON ADMINISTRADOR TO ROL_ADMIN;

GRANT SELECT, INSERT, UPDATE, DELETE ON CAJERO TO ROL_ADMIN;

GRANT SELECT, INSERT, UPDATE, DELETE ON CLIENTE TO ROL_ADMIN;
GRANT SELECT ON CLIENTE TO ROL_CLIENTE;

GRANT SELECT, INSERT, UPDATE, DELETE ON CUENTA TO ROL_ADMIN;
GRANT SELECT ON CUENTA TO ROL_CLIENTE;

GRANT SELECT, INSERT, UPDATE, DELETE ON CUOTA TO ROL_ADMIN;
GRANT SELECT ON CUOTA TO ROL_CLIENTE;

GRANT SELECT, INSERT, UPDATE, DELETE ON PRESTAMO TO ROL_ADMIN;
GRANT SELECT ON PRESTAMO TO ROL_CLIENTE;

GRANT SELECT, INSERT, UPDATE, DELETE ON SUCURSAL TO ROL_ADMIN;

GRANT SELECT, INSERT, UPDATE, DELETE ON TARJETA TO ROL_ADMIN;
GRANT SELECT ON TARJETA TO ROL_CLIENTE;

GRANT SELECT, INSERT, UPDATE, DELETE ON TARJETA_CREDITO TO ROL_ADMIN;
GRANT SELECT ON TARJETA_CREDITO TO ROL_CLIENTE;

GRANT SELECT, INSERT, UPDATE, DELETE ON TARJETA_DEBITO TO ROL_ADMIN;
GRANT SELECT ON TARJETA_DEBITO TO ROL_CLIENTE;

GRANT SELECT, INSERT, UPDATE, DELETE ON TRANSACCION TO ROL_ADMIN;
GRANT SELECT ON TRANSACCION TO ROL_CLIENTE;



--Paquetes 

create or replace PACKAGE pkg_administrador AS

  -- Devuelve ID del admin si login es correcto, sino NULL
  FUNCTION fn_login_admin(
    p_email       IN VARCHAR2,
    p_contrasena  IN VARCHAR2
  ) RETURN NUMBER;

END pkg_administrador;

create or replace PACKAGE BODY pkg_administrador AS

  FUNCTION fn_login_admin(
    p_email       IN VARCHAR2,
    p_contrasena  IN VARCHAR2
  ) RETURN NUMBER IS
    v_id NUMBER;
  BEGIN
    SELECT id_administrador
    INTO v_id
    FROM administrador
    WHERE UPPER(email_administrador) = UPPER(p_email)
      AND contrasena_administrador = p_contrasena;

    RETURN v_id;

  EXCEPTION
    WHEN NO_DATA_FOUND THEN
      RETURN NULL;
  END fn_login_admin;

END pkg_administrador;


create or replace PACKAGE pkg_clientes AS

  -- =============================
  -- SOLICITUDES DE CUENTA
  -- =============================
  PROCEDURE obtener_solicitudes(
    result OUT SYS_REFCURSOR
  );

  -- =============================
  -- CONSTANTES
  -- =============================
  c_estado_activo   CONSTANT VARCHAR2(10) := 'ACTIVO';
  c_estado_inactivo CONSTANT VARCHAR2(10) := 'INACTIVO';

  -- =============================
  -- FUNCIONES UTILITARIAS
  -- =============================
  FUNCTION generar_id_cliente RETURN VARCHAR2;
  FUNCTION existe_cliente(p_id_cliente IN VARCHAR2) RETURN BOOLEAN;
  FUNCTION email_disponible(
    p_email      IN VARCHAR2,
    p_excluir_id IN VARCHAR2 DEFAULT NULL
  ) RETURN BOOLEAN;

  -- =============================
  -- CRUD CLIENTE
  -- =============================
  PROCEDURE crear_cliente(
    p_nombre_cliente        IN VARCHAR2,
    p_email_cliente         IN VARCHAR2,
    p_telefono_cliente      IN VARCHAR2,
    p_cedula_cliente        IN NUMBER,
    p_estado_cliente        IN VARCHAR2 DEFAULT c_estado_activo,
    o_id_cliente            OUT VARCHAR2
  );

  PROCEDURE actualizar_datos_cliente(
    p_id_cliente       IN VARCHAR2,
    p_nombre_cliente   IN VARCHAR2,
    p_email_cliente    IN VARCHAR2,
    p_telefono_cliente IN VARCHAR2,
    p_cedula_cliente   IN NUMBER
  );

  PROCEDURE actualizar_correo_cliente(
    p_id_cliente    IN VARCHAR2,
    p_email_cliente IN VARCHAR2
  );

  PROCEDURE actualizar_nombre_cliente(
    p_id_cliente       IN VARCHAR2,
    p_nombre_cliente   IN VARCHAR2
  );

  PROCEDURE actualizar_telefono_cliente(
    p_id_cliente       IN VARCHAR2,
    p_telefono_cliente IN VARCHAR2
  );

  PROCEDURE actualizar_cedula_cliente(
    p_id_cliente     IN VARCHAR2,
    p_cedula_cliente IN NUMBER
  );

  PROCEDURE eliminar_cliente(
    p_id_cliente IN VARCHAR2
  );

  -- =============================
  -- OPERACIONES DE CUENTAS
  -- =============================
  PROCEDURE crear_cuenta_cliente(
    p_id_cliente       IN VARCHAR2,
    p_id_administrador IN NUMBER,
    o_id_cuenta        OUT NUMBER
  );



END pkg_clientes;

create or replace PACKAGE BODY pkg_clientes AS

  -----------------------------------------------------------------
  -- SOLICITUDES: clientes que aún NO tienen cuenta
  -----------------------------------------------------------------
  PROCEDURE obtener_solicitudes(
    result OUT SYS_REFCURSOR
  ) IS
  BEGIN
      OPEN result FOR
          SELECT 
              c.id_cliente,
              c.nombre_cliente,
              c.email_cliente,
              c.telefono_cliente,
              c.cedula_cliente,
              TO_CHAR(c.fecha_registro_cliente, 'YYYY-MM-DD HH24:MI') AS fecha_registro
          FROM cliente c
          WHERE NOT EXISTS (
              SELECT 1 FROM cuenta cu
              WHERE cu.id_cliente = c.id_cliente
          )
          ORDER BY c.fecha_registro_cliente DESC;
  END obtener_solicitudes;

  -----------------------------------------------------------------
  -- VALIDACIONES Y HELPERS
  -----------------------------------------------------------------
  PROCEDURE validar_estado(p_estado IN VARCHAR2) IS
  BEGIN
    IF p_estado NOT IN (c_estado_activo, c_estado_inactivo) THEN
      raise_application_error(-20001, 'Estado inválido.');
    END IF;
  END;

  PROCEDURE validar_email(p_email IN VARCHAR2) IS
  BEGIN
    IF p_email IS NULL OR
       NOT REGEXP_LIKE(p_email, '^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$') THEN
      raise_application_error(-20002, 'Email con formato inválido.');
    END IF;
  END;

  PROCEDURE asegurar_cliente_existe(p_id_cliente IN VARCHAR2) IS
    v_dummy NUMBER;
  BEGIN
    SELECT 1 INTO v_dummy
    FROM cliente
    WHERE id_cliente = p_id_cliente;
  EXCEPTION WHEN NO_DATA_FOUND THEN
    raise_application_error(-20003, 'Cliente no existe: ' || p_id_cliente);
  END;

  -----------------------------------------------------------------
  -- FUNCTION generar_id_cliente
  -----------------------------------------------------------------
  FUNCTION generar_id_cliente RETURN VARCHAR2 IS
    v_max NUMBER;
  BEGIN
    SELECT NVL(MAX(TO_NUMBER(SUBSTR(id_cliente, 4))), 0)
    INTO v_max
    FROM cliente
    WHERE REGEXP_LIKE(id_cliente, '^CLI[0-9]{3}$');

    RETURN 'CLI' || LPAD(v_max + 1, 3, '0');
  END;

  -----------------------------------------------------------------
  FUNCTION existe_cliente(p_id_cliente IN VARCHAR2) RETURN BOOLEAN IS
    v_cnt NUMBER;
  BEGIN
    SELECT COUNT(*) INTO v_cnt
    FROM cliente
    WHERE id_cliente = p_id_cliente;
    RETURN v_cnt > 0;
  END;

  -----------------------------------------------------------------
  FUNCTION email_disponible(
    p_email      IN VARCHAR2,
    p_excluir_id IN VARCHAR2 DEFAULT NULL
  ) RETURN BOOLEAN IS
    v_cnt NUMBER;
  BEGIN
    IF p_excluir_id IS NULL THEN
        SELECT COUNT(*) INTO v_cnt
        FROM cliente
        WHERE UPPER(email_cliente) = UPPER(p_email);
    ELSE
        SELECT COUNT(*) INTO v_cnt
        FROM cliente
        WHERE UPPER(email_cliente) = UPPER(p_email)
          AND id_cliente <> p_excluir_id;
    END IF;

    RETURN v_cnt = 0;
  END;

  -----------------------------------------------------------------
  -- PROCEDURE crear_cliente
  -----------------------------------------------------------------
  PROCEDURE crear_cliente(
    p_nombre_cliente        IN VARCHAR2,
    p_email_cliente         IN VARCHAR2,
    p_telefono_cliente      IN VARCHAR2,
    p_cedula_cliente        IN NUMBER,
    p_estado_cliente        IN VARCHAR2,
    o_id_cliente            OUT VARCHAR2
  ) IS
    v_id VARCHAR2(10);
    v_cnt NUMBER;
  BEGIN
    validar_estado(p_estado_cliente);
    validar_email(p_email_cliente);

    -- Email único
    IF NOT email_disponible(p_email_cliente) THEN
      raise_application_error(-20004, 'Email ya existe.');
    END IF;

    -- Cédula única
    SELECT COUNT(*) INTO v_cnt
    FROM cliente
    WHERE cedula_cliente = p_cedula_cliente;

    IF v_cnt > 0 THEN
      raise_application_error(-20005, 'Cédula ya existe.');
    END IF;

    v_id := generar_id_cliente;

    INSERT INTO cliente(
      id_cliente,
      nombre_cliente,
      email_cliente,
      telefono_cliente,
      estado_cliente,
      fecha_registro_cliente,
      cedula_cliente
    ) VALUES (
      v_id,
      p_nombre_cliente,
      p_email_cliente,
      p_telefono_cliente,
      p_estado_cliente,
      SYSTIMESTAMP,
      p_cedula_cliente
    );

    o_id_cliente := v_id;
  END;

  -----------------------------------------------------------------
  PROCEDURE actualizar_datos_cliente(
    p_id_cliente       IN VARCHAR2,
    p_nombre_cliente   IN VARCHAR2,
    p_email_cliente    IN VARCHAR2,
    p_telefono_cliente IN VARCHAR2,
    p_cedula_cliente   IN NUMBER
  ) IS
  BEGIN
    asegurar_cliente_existe(p_id_cliente);
    validar_email(p_email_cliente);

    -- Email único
    IF NOT email_disponible(p_email_cliente, p_id_cliente) THEN
      raise_application_error(-20006, 'Email ya está asignado.');
    END IF;

    UPDATE cliente
    SET nombre_cliente   = p_nombre_cliente,
        email_cliente    = p_email_cliente,
        telefono_cliente = p_telefono_cliente,
        cedula_cliente   = p_cedula_cliente
    WHERE id_cliente = p_id_cliente;
  END;

  -----------------------------------------------------------------
  PROCEDURE actualizar_correo_cliente(
    p_id_cliente    IN VARCHAR2,
    p_email_cliente IN VARCHAR2
  ) IS
  BEGIN
    asegurar_cliente_existe(p_id_cliente);
    validar_email(p_email_cliente);

    IF NOT email_disponible(p_email_cliente, p_id_cliente) THEN
      raise_application_error(-20007, 'Email duplicado.');
    END IF;

    UPDATE cliente
    SET email_cliente = p_email_cliente
    WHERE id_cliente = p_id_cliente;
  END;

  -----------------------------------------------------------------
  PROCEDURE actualizar_nombre_cliente(
    p_id_cliente       IN VARCHAR2,
    p_nombre_cliente   IN VARCHAR2
  ) IS
  BEGIN
    asegurar_cliente_existe(p_id_cliente);

    UPDATE cliente
    SET nombre_cliente = p_nombre_cliente
    WHERE id_cliente = p_id_cliente;
  END;

  -----------------------------------------------------------------
  PROCEDURE actualizar_telefono_cliente(
    p_id_cliente       IN VARCHAR2,
    p_telefono_cliente IN VARCHAR2
  ) IS
  BEGIN
    asegurar_cliente_existe(p_id_cliente);

    UPDATE cliente
    SET telefono_cliente = p_telefono_cliente
    WHERE id_cliente = p_id_cliente;
  END;

  -----------------------------------------------------------------
  PROCEDURE actualizar_cedula_cliente(
    p_id_cliente     IN VARCHAR2,
    p_cedula_cliente IN NUMBER
  ) IS
    v_cnt NUMBER;
  BEGIN
    asegurar_cliente_existe(p_id_cliente);

    SELECT COUNT(*) INTO v_cnt
    FROM cliente
    WHERE cedula_cliente = p_cedula_cliente
      AND id_cliente <> p_id_cliente;

    IF v_cnt > 0 THEN
      raise_application_error(-20008, 'Cédula duplicada.');
    END IF;

    UPDATE cliente
    SET cedula_cliente = p_cedula_cliente
    WHERE id_cliente = p_id_cliente;
  END;

  -----------------------------------------------------------------
  PROCEDURE eliminar_cliente(
    p_id_cliente IN VARCHAR2
  ) IS
    v_cnt NUMBER;
  BEGIN
    asegurar_cliente_existe(p_id_cliente);

    SELECT COUNT(*) INTO v_cnt
    FROM cuenta
    WHERE id_cliente = p_id_cliente;

    IF v_cnt > 0 THEN
      raise_application_error(-20009,'Cliente tiene cuentas.');
    END IF;

    UPDATE cliente
    SET estado_cliente = c_estado_inactivo
    WHERE id_cliente = p_id_cliente;
  END;

  -----------------------------------------------------------------
  -- CREAR CUENTA DESDE CLIENTES (para solicitudes)
  -----------------------------------------------------------------
  PROCEDURE crear_cuenta_cliente(
    p_id_cliente       IN VARCHAR2,
    p_id_administrador IN NUMBER,
    o_id_cuenta        OUT NUMBER
  ) IS
    v_exists NUMBER;
    v_cedula NUMBER;
  BEGIN
    asegurar_cliente_existe(p_id_cliente);

    SELECT COUNT(*) INTO v_exists
    FROM administrador
    WHERE id_administrador = p_id_administrador;

    IF v_exists = 0 THEN
      raise_application_error(-20010,'Administrador no existe.');
    END IF;

    SELECT cedula_cliente INTO v_cedula
    FROM cliente
    WHERE id_cliente = p_id_cliente;

    SELECT NVL(MAX(id_cuenta),0)+1
    INTO o_id_cuenta
    FROM cuenta;

    INSERT INTO cuenta(
      id_cuenta,
      id_cliente,
      id_administrador,
      saldo_cuenta,
      fecha_creacion_cuenta,
      estado_cuenta,
      contrasena_cuenta
    ) VALUES (
      o_id_cuenta,
      p_id_cliente,
      p_id_administrador,
      0,
      SYSTIMESTAMP,
      'activa',
      v_cedula
    );
  END;

  -----------------------------------------------------------------
 

END pkg_clientes;








create or replace PACKAGE pkg_cuenta AS

    FUNCTION fn_login_cuenta(
        p_email       IN VARCHAR2,
        p_contrasena  IN VARCHAR2
    ) RETURN NUMBER;

    FUNCTION generar_id_cuenta RETURN NUMBER;

    PROCEDURE crear_cuenta(
        p_id_cliente       IN VARCHAR2,
        p_id_administrador IN NUMBER,
        o_id_cuenta        OUT NUMBER
    );

    PROCEDURE consultar_cuentas_por_cliente(
        p_id_cliente IN VARCHAR2,
        result OUT SYS_REFCURSOR
    );

    PROCEDURE consultar_cuentas_por_id(
        p_id_cuenta IN NUMBER,
        result OUT SYS_REFCURSOR
    );

    PROCEDURE actualizar_cuenta(
        p_id_cuenta   IN NUMBER,
        p_saldo       IN NUMBER DEFAULT NULL,
        p_estado      IN VARCHAR2 DEFAULT NULL
    );

    PROCEDURE cambiar_saldo(
        p_id_cuenta IN NUMBER,
        p_monto     IN NUMBER
    );

    PROCEDURE eliminar_cuenta(
        p_id_cuenta IN NUMBER
    );

    PROCEDURE cambiar_contrasena(
        p_id_cuenta        IN NUMBER,
        p_nueva_contrasena IN VARCHAR2
    );

END pkg_cuenta;

create or replace PACKAGE BODY pkg_cuenta AS

    -------------------------------------------------------------------
    -- FUNCIÓN: fn_login_cuenta
    -- Valida email del CLIENTE + contraseña de la CUENTA
    -------------------------------------------------------------------
    FUNCTION fn_login_cuenta(
      p_email       IN VARCHAR2,
      p_contrasena  IN VARCHAR2
    ) RETURN NUMBER IS
      v_id_cuenta NUMBER;
    BEGIN
      SELECT c.id_cuenta
      INTO v_id_cuenta
      FROM cuenta c
      JOIN cliente cli
        ON cli.id_cliente = c.id_cliente
      WHERE UPPER(cli.email_cliente) = UPPER(p_email)
        AND c.contrasena_cuenta = p_contrasena
        AND c.estado_cuenta = 'activa';

      RETURN v_id_cuenta;

    EXCEPTION
      WHEN NO_DATA_FOUND THEN
        RETURN NULL;
    END fn_login_cuenta;


    -------------------------------------------------------------------
    -- Función: generar_id_cuenta
    -------------------------------------------------------------------
    FUNCTION generar_id_cuenta RETURN NUMBER IS
      v_id NUMBER;
    BEGIN
      SELECT NVL(MAX(id_cuenta), 0) + 1
      INTO v_id
      FROM cuenta;

      RETURN v_id;
    END generar_id_cuenta;


    -------------------------------------------------------------------
    -- Procedimiento: crear_cuenta
    -------------------------------------------------------------------
    PROCEDURE crear_cuenta(
      p_id_cliente       IN VARCHAR2,
      p_id_administrador IN NUMBER,
      o_id_cuenta        OUT NUMBER
    ) IS
      v_cliente_count NUMBER;
      v_admin_count   NUMBER;
      v_cedula_cliente NUMBER;
    BEGIN

      SELECT COUNT(*) INTO v_cliente_count
      FROM cliente
      WHERE id_cliente = p_id_cliente;

      IF v_cliente_count = 0 THEN
        RAISE_APPLICATION_ERROR(-20001, 'El cliente no existe: ' || p_id_cliente);
      END IF;

      SELECT COUNT(*) INTO v_admin_count
      FROM administrador
      WHERE id_administrador = p_id_administrador;

      IF v_admin_count = 0 THEN
        RAISE_APPLICATION_ERROR(-20002, 'El administrador no existe: ' || p_id_administrador);
      END IF;

      SELECT cedula_cliente INTO v_cedula_cliente
      FROM cliente
      WHERE id_cliente = p_id_cliente;

      o_id_cuenta := generar_id_cuenta();

      INSERT INTO cuenta(
        id_cuenta,
        id_cliente,
        id_administrador,
        saldo_cuenta,
        fecha_creacion_cuenta,
        estado_cuenta,
        contrasena_cuenta,
        fecha_ultima_transaccion_cuenta
      ) VALUES (
        o_id_cuenta,
        p_id_cliente,
        p_id_administrador,
        0,
        SYSTIMESTAMP,
        'activa',
        v_cedula_cliente,
        NULL
      );

    END crear_cuenta;


    -------------------------------------------------------------------
    -- Procedimiento: consultar_cuentas_por_cliente
    -------------------------------------------------------------------
    PROCEDURE consultar_cuentas_por_cliente(
    p_id_cliente IN VARCHAR2,
    result OUT SYS_REFCURSOR
) IS
BEGIN
    OPEN result FOR
        SELECT 
            id_cuenta,
            saldo_cuenta,
            estado_cuenta,
            fecha_creacion_cuenta
        FROM cuenta
        WHERE id_cliente = p_id_cliente
        ORDER BY id_cuenta;
END consultar_cuentas_por_cliente;


    -------------------------------------------------------------------
    -- Procedimiento: consultar_cuentas_por_id
    -------------------------------------------------------------------
    PROCEDURE consultar_cuentas_por_id(
    p_id_cuenta IN NUMBER,
    result OUT SYS_REFCURSOR
) IS
BEGIN
    OPEN result FOR
        SELECT 
            id_cuenta,
            id_cliente,
            saldo_cuenta,
            estado_cuenta,
            fecha_creacion_cuenta,
            fecha_ultima_transaccion_cuenta
        FROM cuenta
        WHERE id_cuenta = p_id_cuenta;
END consultar_cuentas_por_id;


    -------------------------------------------------------------------
    -- Procedimiento: actualizar_cuenta
    -------------------------------------------------------------------
    PROCEDURE actualizar_cuenta(
      p_id_cuenta   IN NUMBER,
      p_saldo       IN NUMBER DEFAULT NULL,
      p_estado      IN VARCHAR2 DEFAULT NULL
    ) IS
      v_count NUMBER;
    BEGIN

      SELECT COUNT(*) INTO v_count
      FROM cuenta
      WHERE id_cuenta = p_id_cuenta;

      IF v_count = 0 THEN
        RAISE_APPLICATION_ERROR(-20003,'La cuenta no existe.');
      END IF;

      IF p_estado IS NOT NULL AND p_estado NOT IN ('activa','inactiva') THEN
        RAISE_APPLICATION_ERROR(-20004,'Estado inválido para cuenta.');
      END IF;

      UPDATE cuenta
      SET saldo_cuenta = NVL(p_saldo, saldo_cuenta),
          estado_cuenta = NVL(p_estado, estado_cuenta)
      WHERE id_cuenta = p_id_cuenta;

    END actualizar_cuenta;


    -------------------------------------------------------------------
    -- Procedimiento: cambiar_saldo
    -------------------------------------------------------------------
    PROCEDURE cambiar_saldo(
      p_id_cuenta IN NUMBER,
      p_monto     IN NUMBER
    ) IS
      v_saldo NUMBER;
      v_count NUMBER;
    BEGIN
      -- Validar que la cuenta existe
      SELECT COUNT(*) INTO v_count
      FROM cuenta
      WHERE id_cuenta = p_id_cuenta;

      IF v_count = 0 THEN
        RAISE_APPLICATION_ERROR(-20008, 'La cuenta no existe.');
      END IF;

      -- Obtener saldo actual
      SELECT saldo_cuenta INTO v_saldo
      FROM cuenta
      WHERE id_cuenta = p_id_cuenta;

      -- Validar que el saldo no quede negativo
      IF v_saldo + p_monto < 0 THEN
        RAISE_APPLICATION_ERROR(-20005, 'El saldo no puede quedar negativo.');
      END IF;

      -- Actualizar saldo
      UPDATE cuenta
      SET saldo_cuenta = v_saldo + p_monto,
          fecha_ultima_transaccion_cuenta = SYSTIMESTAMP
      WHERE id_cuenta = p_id_cuenta;

    END cambiar_saldo;


    -------------------------------------------------------------------
    -- Procedimiento: eliminar_cuenta
    -------------------------------------------------------------------
    PROCEDURE eliminar_cuenta(
      p_id_cuenta IN NUMBER
    ) IS
      v_exists NUMBER;
      v_estado VARCHAR2(10);
    BEGIN
      SELECT COUNT(*) INTO v_exists
      FROM cuenta
      WHERE id_cuenta = p_id_cuenta;

      IF v_exists = 0 THEN
        RAISE_APPLICATION_ERROR(-20006,'La cuenta no existe.');
      END IF;

      SELECT estado_cuenta INTO v_estado
      FROM cuenta
      WHERE id_cuenta = p_id_cuenta;

      IF v_estado = 'inactiva' THEN
        DBMS_OUTPUT.PUT_LINE('La cuenta ya estaba inactiva.');
        RETURN;
      END IF;

      UPDATE cuenta
      SET estado_cuenta = 'inactiva',
          fecha_ultima_transaccion_cuenta = SYSTIMESTAMP
      WHERE id_cuenta = p_id_cuenta;

    END eliminar_cuenta;


    -------------------------------------------------------------------
    -- Procedimiento: cambiar_contrasena
    -------------------------------------------------------------------
    PROCEDURE cambiar_contrasena(
      p_id_cuenta        IN NUMBER,
      p_nueva_contrasena IN VARCHAR2
    ) IS
      v_count NUMBER;
    BEGIN

      SELECT COUNT(*) INTO v_count
      FROM cuenta
      WHERE id_cuenta = p_id_cuenta;

      IF v_count = 0 THEN
        RAISE_APPLICATION_ERROR(-20007,'La cuenta no existe.');
      END IF;

      UPDATE cuenta
      SET contrasena_cuenta = p_nueva_contrasena
      WHERE id_cuenta = p_id_cuenta;

    END cambiar_contrasena;

END pkg_cuenta;


create or replace PACKAGE pkg_sucursal AS
    
    PROCEDURE pr_listar_sucursales(
        o_cursor OUT SYS_REFCURSOR
    );
    
    FUNCTION fn_crear_sucursal(
        p_nombre      IN VARCHAR2,
        p_direccion   IN VARCHAR2,
        p_telefono    IN VARCHAR2,
        p_id_admin    IN NUMBER
    ) RETURN NUMBER;

    -- Editar estado
    PROCEDURE pr_editar_estado(
        p_id_sucursal IN NUMBER,
        p_estado      IN VARCHAR2
    );

    -- Eliminar sucursal
    PROCEDURE pr_eliminar_sucursal(
        p_id_sucursal IN NUMBER
    );



END pkg_sucursal;

create or replace PACKAGE BODY pkg_sucursal AS
    PROCEDURE pr_listar_sucursales(
        o_cursor OUT SYS_REFCURSOR
    )
    IS
    BEGIN
        OPEN o_cursor FOR
            SELECT 
                id_sucursal,
                nombre_sucursal,
                direccion_sucursal,
                telefono_sucursal,
                estado_sucursal,
                id_administrador
            FROM sucursal
            ORDER BY id_sucursal;
    END pr_listar_sucursales;
    
    
        ---------------------------------------------------------------------
    -- FUNCIÓN: CREAR SUCURSAL
    -- Compatible con servicio C#
    ---------------------------------------------------------------------
    FUNCTION fn_crear_sucursal(
        p_nombre      IN VARCHAR2,
        p_direccion   IN VARCHAR2,
        p_telefono    IN VARCHAR2,
        p_id_admin    IN NUMBER
    ) RETURN NUMBER
    IS
        PRAGMA AUTONOMOUS_TRANSACTION;

        v_new_id NUMBER;
        v_admin NUMBER;
        v_count NUMBER;

        v_nombre    VARCHAR2(200);
        v_direccion VARCHAR2(200);
        v_telefono  VARCHAR2(50);
    BEGIN
        -----------------------------------------------------------------
        -- 1. Limpiar datos
        -----------------------------------------------------------------
        v_nombre :=
            TRIM(REPLACE(REPLACE(p_nombre, CHR(10), ''), CHR(13), ''));

        v_direccion :=
            TRIM(REPLACE(REPLACE(p_direccion, CHR(10), ''), CHR(13), ''));

        v_telefono :=
            TRIM(REPLACE(REPLACE(p_telefono, CHR(10), ''), CHR(13), ''));

        -----------------------------------------------------------------
        -- 2. Si p_id_admin es NULL → usar administrador por defecto: 1
        -----------------------------------------------------------------
        v_admin := NVL(p_id_admin, 1);

        -----------------------------------------------------------------
        -- 3. Validar que el administrador existe
        -----------------------------------------------------------------
        SELECT COUNT(*) INTO v_count
        FROM administrador
        WHERE id_administrador = v_admin;

        IF v_count = 0 THEN
            RAISE_APPLICATION_ERROR(-20010,
                'El administrador con ID ' || v_admin || ' no existe.');
        END IF;

        -----------------------------------------------------------------
        -- 4. Generar ID nuevo
        -----------------------------------------------------------------
        SELECT NVL(MAX(id_sucursal), 0) + 1 INTO v_new_id
        FROM sucursal;

        -----------------------------------------------------------------
        -- 5. Insertar sucursal
        -----------------------------------------------------------------
        INSERT INTO sucursal(
            id_sucursal,
            nombre_sucursal,
            direccion_sucursal,
            telefono_sucursal,
            estado_sucursal,
            id_administrador
        )
        VALUES (
            v_new_id,
            v_nombre,
            v_direccion,
            v_telefono,
            'abierta',      -- por defecto
            v_admin
        );

        COMMIT;

        RETURN v_new_id;
    EXCEPTION
        WHEN OTHERS THEN
            ROLLBACK;
            RAISE;
    END fn_crear_sucursal;


    ---------------------------------------------------------------------
    -- PROCEDIMIENTO: EDITAR ESTADO
    ---------------------------------------------------------------------
    PROCEDURE pr_editar_estado(
        p_id_sucursal IN NUMBER,
        p_estado      IN VARCHAR2
    )
    IS
        PRAGMA AUTONOMOUS_TRANSACTION;
    BEGIN
        IF LOWER(p_estado) NOT IN ('abierta','cerrada') THEN
            RAISE_APPLICATION_ERROR(-20003,
                ' Estado inválido. Debe ser: abierta / cerrada');
        END IF;

        UPDATE sucursal
        SET estado_sucursal = LOWER(p_estado)
        WHERE id_sucursal = p_id_sucursal;

        IF SQL%ROWCOUNT = 0 THEN
            ROLLBACK;
            RAISE_APPLICATION_ERROR(-20004,
                'No existe la sucursal con ID ' || p_id_sucursal);
        END IF;

        COMMIT;
    END pr_editar_estado;



    ---------------------------------------------------------------------
    -- PROCEDIMIENTO: ELIMINAR SUCURSAL
    ---------------------------------------------------------------------
    PROCEDURE pr_eliminar_sucursal(
        p_id_sucursal IN NUMBER
    )
    IS
        PRAGMA AUTONOMOUS_TRANSACTION;
    BEGIN
        DELETE FROM sucursal
        WHERE id_sucursal = p_id_sucursal;

        IF SQL%ROWCOUNT = 0 THEN
            ROLLBACK;
            RAISE_APPLICATION_ERROR(-20005,
                'No existe la sucursal con ID ' || p_id_sucursal);
        END IF;

        COMMIT;
    END pr_eliminar_sucursal;



END pkg_sucursal;













create or replace PACKAGE pkgc_abonos AS

    -- Registrar abono extraordinario
    PROCEDURE pr_registrar_abono(
        p_id_prestamo     IN NUMBER,
        p_monto_abono     IN NUMBER,
        p_tipo_abono      IN VARCHAR2,
        p_id_cuenta       IN NUMBER,
        o_id_abono        OUT NUMBER,
        o_id_transaccion  OUT NUMBER
    );

    -- Listar abonos de un préstamo
    PROCEDURE pr_listar_abonos(
        p_id_prestamo IN NUMBER,
        o_cursor      OUT SYS_REFCURSOR
    );

END pkgc_abonos;
create or replace PACKAGE pkgc_abonos AS

    -- Registrar abono extraordinario
    PROCEDURE pr_registrar_abono(
        p_id_prestamo     IN NUMBER,
        p_monto_abono     IN NUMBER,
        p_tipo_abono      IN VARCHAR2,
        p_id_cuenta       IN NUMBER,
        o_id_abono        OUT NUMBER,
        o_id_transaccion  OUT NUMBER
    );

    -- Listar abonos de un préstamo
    PROCEDURE pr_listar_abonos(
        p_id_prestamo IN NUMBER,
        o_cursor      OUT SYS_REFCURSOR
    );

END pkgc_abonos;

create or replace PACKAGE pkgc_cajeros AS

    -------------------------------------------------------------------
    -- Crear un cajero
    -------------------------------------------------------------------
    PROCEDURE pr_crear_cajero(
        p_id_sucursal   IN NUMBER,
        p_id_admin      IN NUMBER,
        p_dinero_inicial IN NUMBER,
        o_id_cajero     OUT NUMBER
    );

    -------------------------------------------------------------------
    -- Cambiar estado del cajero (activo / inactivo)
    -------------------------------------------------------------------
    PROCEDURE pr_cambiar_estado_cajero(
        p_id_cajero IN NUMBER,
        p_estado    IN VARCHAR2
    );

    -------------------------------------------------------------------
    -- Recargar dinero en cajero
    -------------------------------------------------------------------
    PROCEDURE pr_recargar_cajero(
        p_id_cajero IN NUMBER,
        p_monto     IN NUMBER
    );

    -------------------------------------------------------------------
    -- Descontar dinero del cajero (retiros)
    -------------------------------------------------------------------
    PROCEDURE pr_descontar_cajero(
        p_id_cajero IN NUMBER,
        p_monto     IN NUMBER
    );

    -------------------------------------------------------------------
    -- Listar cajeros por sucursal
    -------------------------------------------------------------------
    PROCEDURE pr_listar_cajeros(
        p_id_sucursal IN NUMBER,
        o_cursor      OUT SYS_REFCURSOR
    );

END pkgc_cajeros;

create or replace PACKAGE BODY pkgc_cajeros AS


    -------------------------------------------------------------------
    -- CREAR CAJERO
    -------------------------------------------------------------------
    PROCEDURE pr_crear_cajero(
        p_id_sucursal   IN NUMBER,
        p_id_admin      IN NUMBER,
        p_dinero_inicial IN NUMBER,
        o_id_cajero     OUT NUMBER
    )
    IS
        PRAGMA AUTONOMOUS_TRANSACTION;

        v_existe NUMBER;
        v_id NUMBER;
    BEGIN
        ----------------------------------------------------------------
        -- Validar sucursal
        ----------------------------------------------------------------
        SELECT COUNT(*)
        INTO v_existe
        FROM sucursal
        WHERE id_sucursal = p_id_sucursal;

        IF v_existe = 0 THEN
            RAISE_APPLICATION_ERROR(-43001,'La sucursal no existe.');
        END IF;

        ----------------------------------------------------------------
        -- Validar admin
        ----------------------------------------------------------------
        SELECT COUNT(*)
        INTO v_existe
        FROM administrador
        WHERE id_administrador = p_id_admin;

        IF v_existe = 0 THEN
            RAISE_APPLICATION_ERROR(-43002,'El administrador no existe.');
        END IF;

        ----------------------------------------------------------------
        -- Generar ID
        ----------------------------------------------------------------
        SELECT NVL(MAX(id_cajero),0)+1
        INTO v_id
        FROM cajero;

        INSERT INTO cajero(
            id_cajero,
            id_sucursal,
            id_administrador,
            dinero_disponible_cajero,
            estado_cajero,
            fecha_ultima_recarga_cajero
        ) VALUES (
            v_id,
            p_id_sucursal,
            p_id_admin,
            p_dinero_inicial,
            'activo',
            SYSTIMESTAMP
        );

        o_id_cajero := v_id;
        COMMIT;

    EXCEPTION
        WHEN OTHERS THEN
            ROLLBACK;
            RAISE;
    END pr_crear_cajero;



    -------------------------------------------------------------------
    -- CAMBIAR ESTADO DE CAJERO
    -------------------------------------------------------------------
    PROCEDURE pr_cambiar_estado_cajero(
        p_id_cajero IN NUMBER,
        p_estado    IN VARCHAR2
    )
    IS
        PRAGMA AUTONOMOUS_TRANSACTION;

        v_existe NUMBER;
    BEGIN
        IF LOWER(p_estado) NOT IN ('activo','inactivo') THEN
            RAISE_APPLICATION_ERROR(-43010,'Estado inválido.');
        END IF;

        SELECT COUNT(*)
        INTO v_existe
        FROM cajero
        WHERE id_cajero = p_id_cajero;

        IF v_existe = 0 THEN
            RAISE_APPLICATION_ERROR(-43011,'El cajero no existe.');
        END IF;

        UPDATE cajero
        SET estado_cajero = LOWER(p_estado)
        WHERE id_cajero = p_id_cajero;

        COMMIT;
    EXCEPTION
        WHEN OTHERS THEN
            ROLLBACK;
            RAISE;
    END pr_cambiar_estado_cajero;



    -------------------------------------------------------------------
    -- RECARGAR CAJERO
    -------------------------------------------------------------------
    PROCEDURE pr_recargar_cajero(
        p_id_cajero IN NUMBER,
        p_monto     IN NUMBER
    )
    IS
        PRAGMA AUTONOMOUS_TRANSACTION;

        v_existe NUMBER;
    BEGIN
        IF p_monto <= 0 THEN
            RAISE_APPLICATION_ERROR(-43020,'El monto debe ser mayor a 0.');
        END IF;

        SELECT COUNT(*)
        INTO v_existe
        FROM cajero
        WHERE id_cajero = p_id_cajero;

        IF v_existe = 0 THEN
            RAISE_APPLICATION_ERROR(-43021,'El cajero no existe.');
        END IF;

        UPDATE cajero
        SET dinero_disponible_cajero = dinero_disponible_cajero + p_monto,
            fecha_ultima_recarga_cajero = SYSTIMESTAMP
        WHERE id_cajero = p_id_cajero;

        COMMIT;
    EXCEPTION
        WHEN OTHERS THEN
            ROLLBACK;
            RAISE;
    END pr_recargar_cajero;



    -------------------------------------------------------------------
    -- DESCONTAR DINERO DEL CAJERO (RETIROS)
    -------------------------------------------------------------------
    PROCEDURE pr_descontar_cajero(
        p_id_cajero IN NUMBER,
        p_monto     IN NUMBER
    )
    IS
        PRAGMA AUTONOMOUS_TRANSACTION;

        v_dinero NUMBER;
        v_existe NUMBER;
    BEGIN
        SELECT COUNT(*)
        INTO v_existe
        FROM cajero
        WHERE id_cajero = p_id_cajero
          AND estado_cajero = 'activo';

        IF v_existe = 0 THEN
            RAISE_APPLICATION_ERROR(-20007,'El cajero no existe o está inactivo.');
        END IF;

        SELECT dinero_disponible_cajero
        INTO v_dinero
        FROM cajero
        WHERE id_cajero = p_id_cajero;

        IF v_dinero < p_monto THEN
            RAISE_APPLICATION_ERROR(-2008,'El cajero no tiene suficiente efectivo.');
        END IF;

        UPDATE cajero
        SET dinero_disponible_cajero = dinero_disponible_cajero - p_monto
        WHERE id_cajero = p_id_cajero;

        COMMIT;
    EXCEPTION
        WHEN OTHERS THEN
            ROLLBACK;
            RAISE;
    END pr_descontar_cajero;



    -------------------------------------------------------------------
    -- LISTAR CAJEROS POR SUCURSAL
    -------------------------------------------------------------------
    PROCEDURE pr_listar_cajeros(
        p_id_sucursal IN NUMBER,
        o_cursor      OUT SYS_REFCURSOR
    )
    IS
        v_existe NUMBER;
    BEGIN
        SELECT COUNT(*)
        INTO v_existe
        FROM sucursal
        WHERE id_sucursal = p_id_sucursal;

        IF v_existe = 0 THEN
            RAISE_APPLICATION_ERROR(-43040,'La sucursal no existe.');
        END IF;

        OPEN o_cursor FOR
            SELECT 
                id_cajero,
                dinero_disponible_cajero,
                estado_cajero,
                fecha_ultima_recarga_cajero
            FROM cajero
            WHERE id_sucursal = p_id_sucursal
            ORDER BY id_cajero;
    END pr_listar_cajeros;



END pkgc_cajeros;
create or replace PACKAGE pkgc_cuotas AS

    -- Generar todas las cuotas de un préstamo activo
    PROCEDURE pr_generar_cuotas(
        p_id_prestamo IN NUMBER
    );

    -- Pagar una cuota
    PROCEDURE pr_pagar_cuota(
        p_id_cuota        IN NUMBER,
        p_id_cuenta       IN NUMBER,
        o_id_transaccion  OUT NUMBER
    );

    -- Listar cuotas de un préstamo
    PROCEDURE pr_listar_cuotas(
        p_id_prestamo IN NUMBER,
        o_cursor      OUT SYS_REFCURSOR
    );

END pkgc_cuotas;

create or replace PACKAGE BODY pkgc_cuotas AS

    FUNCTION fn_generar_id_cuota RETURN NUMBER IS
        v_id NUMBER;
    BEGIN
        SELECT NVL(MAX(id_cuota),0) + 1
        INTO v_id
        FROM cuota;
        RETURN v_id;
    END fn_generar_id_cuota;


    ----------------------------------------------------------------------
    -- Generar cuotas de un préstamo activo
    ----------------------------------------------------------------------
    PROCEDURE pr_generar_cuotas(
        p_id_prestamo IN NUMBER
    )
    IS
        v_estado      VARCHAR2(20);
        v_plazo       NUMBER;
        v_monto       NUMBER;
        v_valor_cuota NUMBER;
        v_fecha_ini   TIMESTAMP;
        v_i           NUMBER;
        v_id_cuota    NUMBER;
    BEGIN
        -- Traer datos del préstamo (si no existe → NO_DATA_FOUND)
        SELECT estado_prestamo,
               monto_prestamo,
               plazo_prestamo,
               fecha_inicio_prestamo
        INTO  v_estado,
              v_monto,
              v_plazo,
              v_fecha_ini
        FROM prestamo
        WHERE id_prestamo = p_id_prestamo;

        IF v_estado <> 'activo' THEN
            RAISE_APPLICATION_ERROR(-20602,'El préstamo debe estar activo para generar cuotas.');
        END IF;

        IF v_plazo <= 0 THEN
            RAISE_APPLICATION_ERROR(-20603,'El plazo del préstamo no es válido.');
        END IF;

        v_valor_cuota := ROUND(v_monto / v_plazo, 2);

        FOR v_i IN 1 .. v_plazo LOOP
            v_id_cuota := fn_generar_id_cuota();

            INSERT INTO cuota(
                id_cuota,
                id_prestamo,
                numero_cuota,
                monto_de_cuota,
                capital_cuota,
                fecha_de_vencimiento_cuota,
                fecha_de_pago_cuota,
                estado_cuota
            ) VALUES (
                v_id_cuota,
                p_id_prestamo,
                v_i,
                v_valor_cuota,
                v_valor_cuota,
                ADD_MONTHS(v_fecha_ini, v_i),
                NULL,
                'pendiente'
            );
        END LOOP;

    EXCEPTION
        WHEN NO_DATA_FOUND THEN
            -- En vez de tirar ORA-20004 dejamos mensaje y no reventamos todo el flujo
            DBMS_OUTPUT.PUT_LINE('Aviso: préstamo '||p_id_prestamo||' no existe al generar cuotas.');
        WHEN OTHERS THEN
            RAISE;
    END pr_generar_cuotas;


    ----------------------------------------------------------------------
    -- Pagar cuota
    ----------------------------------------------------------------------
    PROCEDURE pr_pagar_cuota(
        p_id_cuota        IN NUMBER,
        p_id_cuenta       IN NUMBER,
        o_id_transaccion  OUT NUMBER
    )
    IS
        v_monto    NUMBER;
        v_estado   VARCHAR2(10);
        v_prestamo NUMBER;
        v_saldo    NUMBER;
        v_existe   NUMBER;
        v_id_trx   NUMBER;
    BEGIN
        -- Validar cuota
        SELECT estado_cuota,
               monto_de_cuota,
               id_prestamo
        INTO  v_estado,
              v_monto,
              v_prestamo
        FROM cuota
        WHERE id_cuota = p_id_cuota;

        IF v_estado = 'pagada' THEN
            RAISE_APPLICATION_ERROR(-20612,'La cuota ya está pagada.');
        END IF;

        -- Validar cuenta
        SELECT COUNT(*)
        INTO v_existe
        FROM cuenta
        WHERE id_cuenta = p_id_cuenta
          AND estado_cuenta = 'activa';

        IF v_existe = 0 THEN
            RAISE_APPLICATION_ERROR(-20613,'La cuenta no existe o no está activa.');
        END IF;

        SELECT saldo_cuenta
        INTO v_saldo
        FROM cuenta
        WHERE id_cuenta = p_id_cuenta;

        IF v_saldo < v_monto THEN
            RAISE_APPLICATION_ERROR(-20614,'Saldo insuficiente para pagar la cuota.');
        END IF;

        -- Transacción
        SELECT NVL(MAX(id_transaccion),0)+1
        INTO v_id_trx
        FROM transaccion;

        INSERT INTO transaccion(
            id_transaccion,
            id_cajero,
            id_tarjeta,
            id_cuenta_origen,
            id_cuenta_destino,
            fecha_transaccion,
            tipo_transaccion,
            monto_transaccion,
            estado_transaccion,
            descripcion_transaccion
        ) VALUES (
            v_id_trx,
            NULL,
            NULL,
            p_id_cuenta,
            NULL,
            SYSTIMESTAMP,
            'cuenta',
            v_monto,
            'completada',
            'Pago cuota '||p_id_cuota||' del préstamo '||v_prestamo
        );

        o_id_transaccion := v_id_trx;

        -- Actualizar cuenta
        UPDATE cuenta
        SET saldo_cuenta = saldo_cuenta - v_monto,
            fecha_ultima_transaccion_cuenta = SYSTIMESTAMP
        WHERE id_cuenta = p_id_cuenta;

        -- Actualizar cuota
        UPDATE cuota
        SET estado_cuota = 'pagada',
            fecha_de_pago_cuota = SYSTIMESTAMP
        WHERE id_cuota = p_id_cuota;

        -- Recalcular saldo del préstamo
        pkgc_prestamos.pr_actualizar_saldo(v_prestamo);

    EXCEPTION
        WHEN NO_DATA_FOUND THEN
            RAISE_APPLICATION_ERROR(-20615,'La cuota no existe.');
        WHEN OTHERS THEN
            RAISE;
    END pr_pagar_cuota;


    ----------------------------------------------------------------------
    -- Listar cuotas
    ----------------------------------------------------------------------
    PROCEDURE pr_listar_cuotas(
        p_id_prestamo IN NUMBER,
        o_cursor      OUT SYS_REFCURSOR
    )
    IS
        v_existe NUMBER;
    BEGIN
        SELECT COUNT(*)
        INTO v_existe
        FROM prestamo
        WHERE id_prestamo = p_id_prestamo;

        IF v_existe = 0 THEN
            RAISE_APPLICATION_ERROR(-20620,'El préstamo no existe.');
        END IF;

        OPEN o_cursor FOR
            SELECT
                id_cuota,
                numero_cuota,
                monto_de_cuota,
                capital_cuota,
                fecha_de_vencimiento_cuota,
                fecha_de_pago_cuota,
                estado_cuota
            FROM cuota
            WHERE id_prestamo = p_id_prestamo
            ORDER BY numero_cuota;
    END pr_listar_cuotas;


END pkgc_cuotas;


create or replace PACKAGE pkgc_prestamos AS

    -- Solicitar préstamo (queda en estado 'solicitado')
    PROCEDURE pr_solicitar_prestamo(
        p_id_cliente     IN VARCHAR2,
        p_id_sucursal    IN NUMBER,
        p_monto          IN NUMBER,
        p_fecha_inicio   IN DATE,
        p_fecha_fin      IN DATE,
        o_id_prestamo    OUT NUMBER
    );

    -- Listar préstamos solicitados por sucursal
    PROCEDURE pr_listar_solicitudes(
        p_id_sucursal IN NUMBER,
        o_cursor      OUT SYS_REFCURSOR
    );

    -- Aceptar préstamo (pasa a 'activo', calcula plazo)
    PROCEDURE pr_aceptar(
        p_id_prestamo IN NUMBER
    );

    -- Rechazar préstamo (estado 'rechazado')
    PROCEDURE pr_rechazar(
        p_id_prestamo IN NUMBER
    );

    -- Recalcular saldo del préstamo (usado por cuotas y abonos)
    PROCEDURE pr_actualizar_saldo(
        p_id_prestamo IN NUMBER
    );

END pkgc_prestamos;
create or replace PACKAGE BODY pkgc_prestamos AS

    ----------------------------------------------------------------------
    -- HELPER: Generar ID de préstamo
    ----------------------------------------------------------------------
    FUNCTION fn_generar_id_prestamo
        RETURN NUMBER
    IS
        v_id NUMBER;
    BEGIN
        SELECT NVL(MAX(id_prestamo),0) + 1
        INTO v_id
        FROM prestamo;

        RETURN v_id;
    END fn_generar_id_prestamo;


    ----------------------------------------------------------------------
    -- Solicitar préstamo  (estado inicial = 'solicitado')
    ----------------------------------------------------------------------
    PROCEDURE pr_solicitar_prestamo(
        p_id_cliente     IN VARCHAR2,
        p_id_sucursal    IN NUMBER,
        p_monto          IN NUMBER,
        p_fecha_inicio   IN DATE,
        p_fecha_fin      IN DATE,
        o_id_prestamo    OUT NUMBER
    )
    IS
        v_existe      NUMBER;
        v_id_prestamo NUMBER;
    BEGIN
        IF p_monto <= 0 THEN
            RAISE_APPLICATION_ERROR(-20001, 'El monto solicitado debe ser mayor que 0.');
        END IF;

        IF p_fecha_fin <= p_fecha_inicio THEN
            RAISE_APPLICATION_ERROR(-20002, 'La fecha fin debe ser mayor que la fecha inicio.');
        END IF;

        -- Cliente existe
        SELECT COUNT(*)
        INTO v_existe
        FROM cliente
        WHERE id_cliente = p_id_cliente;

        IF v_existe = 0 THEN
            RAISE_APPLICATION_ERROR(-20003, 'El cliente no existe.');
        END IF;

        -- Sucursal existe
        SELECT COUNT(*)
        INTO v_existe
        FROM sucursal
        WHERE id_sucursal = p_id_sucursal;

        IF v_existe = 0 THEN
            RAISE_APPLICATION_ERROR(-20004, 'La sucursal no existe.');
        END IF;

        -- Generar ID
        v_id_prestamo := fn_generar_id_prestamo();

        -- Insertar el préstamo como "solicitado"
        INSERT INTO prestamo(
            id_prestamo,
            id_sucursal,
            monto_prestamo,
            tasa_de_interes_prestamo,
            plazo_prestamo,
            fecha_inicio_prestamo,
            fecha_vencimiento_prestamo,
            estado_prestamo,
            fecha_ultimo_pago_prestamo,
            saldo_prestamo,
            id_cliente
        ) VALUES (
            v_id_prestamo,
            p_id_sucursal,
            p_monto,
            0,                 -- aún no se asigna tasa
            0,                 -- aún no se genera plan
            p_fecha_inicio,
            p_fecha_fin,
            'solicitado',      -- estado inicial
            NULL,
            p_monto,           -- saldo inicial
            p_id_cliente
        );

        o_id_prestamo := v_id_prestamo;
    END pr_solicitar_prestamo;


    ----------------------------------------------------------------------
    -- Listar préstamos solicitados en una sucursal
    ----------------------------------------------------------------------
    PROCEDURE pr_listar_solicitudes(
        p_id_sucursal IN NUMBER,
        o_cursor      OUT SYS_REFCURSOR
    )
    IS
        v_existe NUMBER;
    BEGIN
        SELECT COUNT(*)
        INTO v_existe
        FROM sucursal
        WHERE id_sucursal = p_id_sucursal;

        IF v_existe = 0 THEN
            RAISE_APPLICATION_ERROR(-20010, 'La sucursal no existe.');
        END IF;

        OPEN o_cursor FOR
            SELECT
                p.id_prestamo,
                p.id_cliente,
                p.monto_prestamo,
                p.fecha_inicio_prestamo,
                p.fecha_vencimiento_prestamo,
                p.estado_prestamo
            FROM prestamo p
            WHERE p.id_sucursal = p_id_sucursal
              AND p.estado_prestamo = 'solicitado'
            ORDER BY p.fecha_inicio_prestamo DESC;
    END pr_listar_solicitudes;


    ----------------------------------------------------------------------
    -- Aceptar préstamo → pasa a ACTIVO, calcula plazo y tasa
    ----------------------------------------------------------------------
    PROCEDURE pr_aceptar(
        p_id_prestamo IN NUMBER
    )
    IS
        v_existe  NUMBER;
        v_monto   NUMBER;
        v_dias    NUMBER;
    BEGIN
        -- Validar préstamo existe
        SELECT COUNT(*)
        INTO v_existe
        FROM prestamo
        WHERE id_prestamo = p_id_prestamo;

        IF v_existe = 0 THEN
            RAISE_APPLICATION_ERROR(-20020, 'El préstamo no existe.');
        END IF;

        -- Validar estado solicitado
        SELECT COUNT(*)
        INTO v_existe
        FROM prestamo
        WHERE id_prestamo = p_id_prestamo
          AND estado_prestamo = 'solicitado';

        IF v_existe = 0 THEN
            RAISE_APPLICATION_ERROR(-20021, 'El préstamo no está en estado solicitado.');
        END IF;

        -- Calcular plazo en días
        SELECT 
               EXTRACT(DAY FROM (fecha_vencimiento_prestamo - fecha_inicio_prestamo))
             + EXTRACT(HOUR   FROM (fecha_vencimiento_prestamo - fecha_inicio_prestamo)) / 24
             + EXTRACT(MINUTE FROM (fecha_vencimiento_prestamo - fecha_inicio_prestamo)) / 1440
             + EXTRACT(SECOND FROM (fecha_vencimiento_prestamo - fecha_inicio_prestamo)) / 86400
        INTO v_dias
        FROM prestamo
        WHERE id_prestamo = p_id_prestamo;

        IF v_dias <= 0 THEN
            RAISE_APPLICATION_ERROR(-20022, 'El plazo calculado del préstamo no es válido.');
        END IF;

        -- Activar préstamo
        UPDATE prestamo
        SET tasa_de_interes_prestamo = 10,
            plazo_prestamo           = v_dias,
            estado_prestamo          = 'activo',
            saldo_prestamo           = monto_prestamo
        WHERE id_prestamo = p_id_prestamo;
    END pr_aceptar;


    ----------------------------------------------------------------------
    -- Rechazar préstamo
    ----------------------------------------------------------------------
    PROCEDURE pr_rechazar(
        p_id_prestamo IN NUMBER
    )
    IS
        v_existe NUMBER;
    BEGIN
        SELECT COUNT(*)
        INTO v_existe
        FROM prestamo
        WHERE id_prestamo = p_id_prestamo;

        IF v_existe = 0 THEN
            RAISE_APPLICATION_ERROR(-20030, 'El préstamo no existe.');
        END IF;

        UPDATE prestamo
        SET estado_prestamo = 'rechazado'
        WHERE id_prestamo = p_id_prestamo;
    END pr_rechazar;


    ----------------------------------------------------------------------
    -- Recalcular saldo del préstamo (usado por cuotas y abonos)
    ----------------------------------------------------------------------
    PROCEDURE pr_actualizar_saldo(
        p_id_prestamo IN NUMBER
    )
    IS
        v_monto_ini NUMBER;
        v_abonos    NUMBER;
        v_cuotas    NUMBER;
        v_nuevo     NUMBER;
    BEGIN
        -- Saldo inicial (monto del préstamo)
        SELECT monto_prestamo
        INTO v_monto_ini
        FROM prestamo
        WHERE id_prestamo = p_id_prestamo;

        -- Total abonos extraordinarios
        SELECT NVL(SUM(monto_abono), 0)
        INTO v_abonos
        FROM abono_extraordinario
        WHERE id_prestamo = p_id_prestamo;

        -- Total pagado en cuotas
        SELECT NVL(SUM(capital_cuota), 0)
        INTO v_cuotas
        FROM cuota
        WHERE id_prestamo = p_id_prestamo
          AND estado_cuota = 'pagada';

        v_nuevo := v_monto_ini - (v_abonos + v_cuotas);

        IF v_nuevo < 0 THEN
            v_nuevo := 0;
        END IF;

        UPDATE prestamo
        SET saldo_prestamo             = v_nuevo,
            fecha_ultimo_pago_prestamo = SYSTIMESTAMP
        WHERE id_prestamo = p_id_prestamo;
    END pr_actualizar_saldo;


END pkgc_prestamos;
create or replace PACKAGE pkgc_tarjetas AS

    -------------------------------------------------------------------
    -- Crear tarjeta de débito
    -------------------------------------------------------------------
    PROCEDURE pr_crear_tarjeta_debito(
        p_id_cuenta           IN NUMBER,
        p_limite_retiro       IN NUMBER,
        o_id_tarjeta          OUT NUMBER
    );

    -------------------------------------------------------------------
    -- Crear tarjeta de crédito
    -------------------------------------------------------------------
    PROCEDURE pr_crear_tarjeta_credito(
        p_id_cuenta           IN NUMBER,
        p_limite_credito      IN NUMBER,
        p_tasa_interes        IN NUMBER,
        p_cuota_manejo        IN NUMBER,
        p_fecha_corte         IN NUMBER,
        o_id_tarjeta          OUT NUMBER
    );

    -------------------------------------------------------------------
    -- Cambiar estado de tarjeta
    -- Estados válidos: activa, inactiva, bloqueada, suspendida
    -------------------------------------------------------------------
    PROCEDURE pr_cambiar_estado_tarjeta(
        p_id_tarjeta IN NUMBER,
        p_estado     IN VARCHAR2
    );

    -------------------------------------------------------------------
    -- Consultar tarjetas de una cuenta
    -------------------------------------------------------------------
    PROCEDURE pr_listar_tarjetas(
        p_id_cuenta IN NUMBER,
        o_cursor    OUT SYS_REFCURSOR
    );

END pkgc_tarjetas;
create or replace PACKAGE BODY pkgc_tarjetas AS

    -------------------------------------------------------------------
    -- Generar ID para tarjeta
    -------------------------------------------------------------------
    FUNCTION fn_generar_id_tarjeta RETURN NUMBER IS
        v_id NUMBER;
    BEGIN
        SELECT NVL(MAX(id_tarjeta), 0) + 1
        INTO v_id
        FROM tarjeta;
        RETURN v_id;
    END fn_generar_id_tarjeta;



    -------------------------------------------------------------------
    -- Crear TARJETA DE DÉBITO
    -------------------------------------------------------------------
    PROCEDURE pr_crear_tarjeta_debito(
        p_id_cuenta           IN NUMBER,
        p_limite_retiro       IN NUMBER,
        o_id_tarjeta          OUT NUMBER
    )
    IS
        v_existe NUMBER;
        v_id_tarjeta NUMBER;
        v_numero VARCHAR2(25);
        v_cvv     VARCHAR2(3);
    BEGIN
        ----------------------------------------------------------------
        -- Validar cuenta
        ----------------------------------------------------------------
        SELECT COUNT(*)
        INTO v_existe
        FROM cuenta
        WHERE id_cuenta = p_id_cuenta
          AND LOWER(estado_cuenta) = 'activa';

        IF v_existe = 0 THEN
            RAISE_APPLICATION_ERROR(-20101, 'La cuenta no existe o no está activa.');
        END IF;

        ----------------------------------------------------------------
        -- Generar tarjeta
        ----------------------------------------------------------------
        v_id_tarjeta := fn_generar_id_tarjeta();
        v_numero := TO_CHAR(TRUNC(DBMS_RANDOM.VALUE(1000000000000000, 9999999999999999)));
        v_cvv    := LPAD(TRUNC(DBMS_RANDOM.VALUE(1,999)), 3, '0');

        INSERT INTO tarjeta(
            id_tarjeta,
            id_cuenta,
            numero_tarjeta,
            cvv_tarjeta,
            fecha_emision_tarjeta,
            fecha_vencimiento_tarjeta,
            estado_tarjeta
        ) VALUES (
            v_id_tarjeta,
            p_id_cuenta,
            v_numero,
            v_cvv,
            SYSTIMESTAMP,
            ADD_MONTHS(SYSTIMESTAMP, 48),
            'activa'
        );

        INSERT INTO tarjeta_debito(
            id_tarjeta,
            limite_retiro_debito,
            saldo_actual_debito
        ) VALUES (
            v_id_tarjeta,
            p_limite_retiro,
            0
        );

        o_id_tarjeta := v_id_tarjeta;
    END pr_crear_tarjeta_debito;



    -------------------------------------------------------------------
    -- Crear TARJETA DE CRÉDITO
    -------------------------------------------------------------------
    PROCEDURE pr_crear_tarjeta_credito(
        p_id_cuenta           IN NUMBER,
        p_limite_credito      IN NUMBER,
        p_tasa_interes        IN NUMBER,
        p_cuota_manejo        IN NUMBER,
        p_fecha_corte         IN NUMBER,
        o_id_tarjeta          OUT NUMBER
    )
    IS
        v_existe NUMBER;
        v_id_tarjeta NUMBER;
        v_numero VARCHAR2(25);
        v_cvv     VARCHAR2(3);
    BEGIN

        SELECT COUNT(*)
        INTO v_existe
        FROM cuenta
        WHERE id_cuenta = p_id_cuenta
          AND LOWER(estado_cuenta) = 'activa';

        IF v_existe = 0 THEN
            RAISE_APPLICATION_ERROR(-20111, 'La cuenta no existe o no está activa.');
        END IF;

        v_id_tarjeta := fn_generar_id_tarjeta();
        v_numero := TO_CHAR(TRUNC(DBMS_RANDOM.VALUE(1000000000000000, 9999999999999999)));
        v_cvv    := LPAD(TRUNC(DBMS_RANDOM.VALUE(1,999)), 3, '0');

        INSERT INTO tarjeta(
            id_tarjeta,
            id_cuenta,
            numero_tarjeta,
            cvv_tarjeta,
            fecha_emision_tarjeta,
            fecha_vencimiento_tarjeta,
            estado_tarjeta
        ) VALUES (
            v_id_tarjeta,
            p_id_cuenta,
            v_numero,
            v_cvv,
            SYSTIMESTAMP,
            ADD_MONTHS(SYSTIMESTAMP, 48),
            'activa'
        );

        INSERT INTO tarjeta_credito(
            id_tarjeta,
            tasa_interes_credito,
            limite_credito,
            cuota_manejo_credito,
            fecha_pago_credito,
            fecha_corte_credito
        ) VALUES (
            v_id_tarjeta,
            p_tasa_interes,
            p_limite_credito,
            p_cuota_manejo,
            SYSTIMESTAMP,
            p_fecha_corte
        );

        o_id_tarjeta := v_id_tarjeta;
    END pr_crear_tarjeta_credito;



    -------------------------------------------------------------------
    -- Cambiar estado de tarjeta
    -------------------------------------------------------------------
    PROCEDURE pr_cambiar_estado_tarjeta(
        p_id_tarjeta IN NUMBER,
        p_estado     IN VARCHAR2
    )
    IS
        v_existe NUMBER;
    BEGIN
        IF LOWER(p_estado) NOT IN ('activa','inactiva','suspendida','bloqueada') THEN
            RAISE_APPLICATION_ERROR(-20121,'Estado inválido.');
        END IF;

        SELECT COUNT(*)
        INTO v_existe
        FROM tarjeta
        WHERE id_tarjeta = p_id_tarjeta;

        IF v_existe = 0 THEN
            RAISE_APPLICATION_ERROR(-20122,'La tarjeta no existe.');
        END IF;

        UPDATE tarjeta
        SET estado_tarjeta = LOWER(p_estado)
        WHERE id_tarjeta = p_id_tarjeta;
    END pr_cambiar_estado_tarjeta;



    -------------------------------------------------------------------
    -- Listar tarjetas asociadas a una cuenta
    -------------------------------------------------------------------
    PROCEDURE pr_listar_tarjetas(
        p_id_cuenta IN NUMBER,
        o_cursor    OUT SYS_REFCURSOR
    )
    IS
        v_existe NUMBER;
    BEGIN
        SELECT COUNT(*)
        INTO v_existe
        FROM cuenta
        WHERE id_cuenta = p_id_cuenta;

        IF v_existe = 0 THEN
            RAISE_APPLICATION_ERROR(-20130,'La cuenta no existe.');
        END IF;

        OPEN o_cursor FOR
            SELECT
                t.id_tarjeta,
                t.numero_tarjeta,
                t.estado_tarjeta,
                t.fecha_emision_tarjeta,
                t.fecha_vencimiento_tarjeta,
                CASE
                    WHEN tc.id_tarjeta IS NOT NULL THEN 'credito'
                    WHEN td.id_tarjeta IS NOT NULL THEN 'debito'
                END AS tipo_tarjeta
            FROM tarjeta t
            LEFT JOIN tarjeta_credito tc ON t.id_tarjeta = tc.id_tarjeta
            LEFT JOIN tarjeta_debito td  ON t.id_tarjeta = td.id_tarjeta
            WHERE t.id_cuenta = p_id_cuenta
            ORDER BY t.id_tarjeta;
    END pr_listar_tarjetas;

END pkgc_tarjetas;
create or replace PACKAGE pkgc_transacciones AS

    -- TRANSACCIONES ENTRE CUENTAS
    PROCEDURE pr_transferencia(
        p_cuenta_origen  IN NUMBER,
        p_cuenta_destino IN NUMBER,
        p_monto          IN NUMBER,
        o_id_transaccion OUT NUMBER
    );

    -- RETIROS EN CAJERO CON TARJETA DEBITO
    PROCEDURE pr_retiro_debito(
        p_id_tarjeta     IN NUMBER,
        p_id_cajero      IN NUMBER,
        p_monto          IN NUMBER,
        o_id_transaccion OUT NUMBER
    );

    -- RETIROS EN CAJERO CON TARJETA CREDITO
    PROCEDURE pr_retiro_credito(
        p_id_tarjeta     IN NUMBER,
        p_id_cajero      IN NUMBER,
        p_monto          IN NUMBER,
        o_id_transaccion OUT NUMBER
    );

    -- HISTORIAL TRANSACCIONES POR CUENTA
    PROCEDURE pr_historial_cuenta(
        p_id_cuenta IN NUMBER,
        o_cursor    OUT SYS_REFCURSOR
    );

    -- HISTORIAL SOLO TRANSACCIONES DE CAJERO
    PROCEDURE pr_historial_cajero(
        p_id_cuenta IN NUMBER,
        o_cursor    OUT SYS_REFCURSOR
    );

END pkgc_transacciones;
create or replace PACKAGE BODY pkgc_transacciones AS

    ------------------------------------------------------------------
    -- GENERAR ID DE TRANSACCIÓN
    ------------------------------------------------------------------
    FUNCTION fn_generar_id_transaccion
        RETURN NUMBER
    IS
        v_id NUMBER;
    BEGIN
        SELECT NVL(MAX(id_transaccion), 0) + 1
        INTO v_id
        FROM transaccion;

        RETURN v_id;
    END fn_generar_id_transaccion;


    ------------------------------------------------------------------
    -- TRANSFERENCIA ENTRE CUENTAS (SIN DEADLOCK)
    ------------------------------------------------------------------
    PROCEDURE pr_transferencia(
        p_cuenta_origen  IN NUMBER,
        p_cuenta_destino IN NUMBER,
        p_monto          IN NUMBER,
        o_id_transaccion OUT NUMBER
    )
    IS
        v_saldo_origen NUMBER;
        v_estado_origen VARCHAR2(10);
        v_estado_destino VARCHAR2(10);
        v_id_trx NUMBER;
    BEGIN
        IF p_monto <= 0 THEN
            RAISE_APPLICATION_ERROR(-20001, 'El monto debe ser mayor que cero.');
        END IF;

        IF p_cuenta_origen = p_cuenta_destino THEN
            RAISE_APPLICATION_ERROR(-20002, 'La cuenta origen y destino no pueden ser la misma.');
        END IF;

        ------------------------------------------------------------------
        -- BLOQUEO SIEMPRE EN EL MISMO ORDEN → ORIGEN → DESTINO
        ------------------------------------------------------------------
        SELECT estado_cuenta, saldo_cuenta 
        INTO v_estado_origen, v_saldo_origen
        FROM cuenta
        WHERE id_cuenta = p_cuenta_origen
        FOR UPDATE;

        IF v_estado_origen <> 'activa' THEN
            RAISE_APPLICATION_ERROR(-20005, 'La cuenta origen no está activa.');
        END IF;

        SELECT estado_cuenta
        INTO v_estado_destino
        FROM cuenta
        WHERE id_cuenta = p_cuenta_destino
        FOR UPDATE;

        IF v_estado_destino <> 'activa' THEN
            RAISE_APPLICATION_ERROR(-20006, 'La cuenta destino no está activa.');
        END IF;

        IF v_saldo_origen < p_monto THEN
            RAISE_APPLICATION_ERROR(-20007, 'Saldo insuficiente.');
        END IF;

        UPDATE cuenta
        SET saldo_cuenta = saldo_cuenta - p_monto,
            fecha_ultima_transaccion_cuenta = SYSTIMESTAMP
        WHERE id_cuenta = p_cuenta_origen;

        UPDATE cuenta
        SET saldo_cuenta = saldo_cuenta + p_monto,
            fecha_ultima_transaccion_cuenta = SYSTIMESTAMP
        WHERE id_cuenta = p_cuenta_destino;

        v_id_trx := fn_generar_id_transaccion;

        INSERT INTO transaccion(
            id_transaccion, id_cajero, id_tarjeta,
            id_cuenta_origen, id_cuenta_destino,
            fecha_transaccion, tipo_transaccion,
            monto_transaccion, estado_transaccion,
            descripcion_transaccion
        ) VALUES (
            v_id_trx,
            NULL, NULL,
            p_cuenta_origen, p_cuenta_destino,
            SYSTIMESTAMP, 'cuenta',
            p_monto, 'completada',
            'Transferencia entre cuentas'
        );

        o_id_transaccion := v_id_trx;
    END pr_transferencia;


    ------------------------------------------------------------------
    -- RETIRO CON TARJETA DE DÉBITO (SIN DEADLOCK)
    ------------------------------------------------------------------
    PROCEDURE pr_retiro_debito(
        p_id_tarjeta     IN NUMBER,
        p_id_cajero      IN NUMBER,
        p_monto          IN NUMBER,
        o_id_transaccion OUT NUMBER
    )
    IS
        v_id_cuenta NUMBER;
        v_saldo NUMBER;
        v_limite NUMBER;
        v_dinero_cajero NUMBER;
        v_estado_tar varchar2(10);
        v_estado_caj varchar2(10);
        v_trx NUMBER;
    BEGIN
        IF p_monto <= 0 THEN
            RAISE_APPLICATION_ERROR(-20101, 'El monto debe ser mayor que cero.');
        END IF;

        SELECT id_cuenta, estado_tarjeta
        INTO v_id_cuenta, v_estado_tar
        FROM tarjeta
        WHERE id_tarjeta = p_id_tarjeta
        FOR UPDATE;

        IF v_estado_tar <> 'activa' THEN
            RAISE_APPLICATION_ERROR(-20103, 'La tarjeta no está activa.');
        END IF;

        SELECT limite_retiro_debito
        INTO v_limite
        FROM tarjeta_debito
        WHERE id_tarjeta = p_id_tarjeta;

        IF p_monto > v_limite THEN
            RAISE_APPLICATION_ERROR(-20104, 'Excede el límite de retiro.');
        END IF;

        ------------------------------------------------------------------
        -- BLOQUEO DE CUENTA → LUEGO CAJERO (orden consistente)
        ------------------------------------------------------------------
        SELECT saldo_cuenta
        INTO v_saldo
        FROM cuenta
        WHERE id_cuenta = v_id_cuenta
        FOR UPDATE;

        IF v_saldo < p_monto THEN
            RAISE_APPLICATION_ERROR(-20105, 'Saldo insuficiente.');
        END IF;

        SELECT dinero_disponible_cajero, estado_cajero
        INTO v_dinero_cajero, v_estado_caj
        FROM cajero
        WHERE id_cajero = p_id_cajero
        FOR UPDATE;

        IF v_estado_caj <> 'activo' THEN
            RAISE_APPLICATION_ERROR(-20106, 'Cajero fuera de servicio.');
        END IF;

        IF v_dinero_cajero < p_monto THEN
            RAISE_APPLICATION_ERROR(-20107, 'Cajero sin suficiente dinero.');
        END IF;

        UPDATE cuenta
        SET saldo_cuenta = saldo_cuenta - p_monto
        WHERE id_cuenta = v_id_cuenta;

        UPDATE cajero
        SET dinero_disponible_cajero = dinero_disponible_cajero - p_monto
        WHERE id_cajero = p_id_cajero;

        v_trx := fn_generar_id_transaccion;

        INSERT INTO transaccion VALUES (
            v_trx, p_id_cajero, p_id_tarjeta,
            v_id_cuenta, NULL,
            SYSTIMESTAMP, 'cajero',
            p_monto, 'completada',
            'Retiro débito'
        );

        o_id_transaccion := v_trx;
    END pr_retiro_debito;


    ------------------------------------------------------------------
    -- RETIRO CON TARJETA DE CRÉDITO (SIN DEADLOCK)
    ------------------------------------------------------------------
    PROCEDURE pr_retiro_credito(
        p_id_tarjeta     IN NUMBER,
        p_id_cajero      IN NUMBER,
        p_monto          IN NUMBER,
        o_id_transaccion OUT NUMBER
    )
    IS
        v_estado_tar VARCHAR2(10);
        v_limite NUMBER;
        v_usado NUMBER;
        v_disp NUMBER;
        v_cajero NUMBER;
        v_estado_caj VARCHAR2(10);
        v_trx NUMBER;
    BEGIN
        IF p_monto <= 0 THEN
            RAISE_APPLICATION_ERROR(-20201, 'Monto inválido.');
        END IF;

        SELECT estado_tarjeta
        INTO v_estado_tar
        FROM tarjeta
        WHERE id_tarjeta = p_id_tarjeta
        FOR UPDATE;

        IF v_estado_tar <> 'activa' THEN
            RAISE_APPLICATION_ERROR(-20203, 'Tarjeta inactiva.');
        END IF;

        SELECT limite_credito
        INTO v_limite
        FROM tarjeta_credito
        WHERE id_tarjeta = p_id_tarjeta;

        SELECT NVL(SUM(monto_transaccion), 0)
        INTO v_usado
        FROM transaccion
        WHERE id_tarjeta = p_id_tarjeta
          AND tipo_transaccion = 'cajero';

        v_disp := v_limite - v_usado;

        IF p_monto > v_disp THEN
            RAISE_APPLICATION_ERROR(-20204, 'Cupo insuficiente.');
        END IF;

        SELECT dinero_disponible_cajero, estado_cajero
        INTO v_cajero, v_estado_caj
        FROM cajero
        WHERE id_cajero = p_id_cajero
        FOR UPDATE;

        IF v_estado_caj <> 'activo' THEN
            RAISE_APPLICATION_ERROR(-20205, 'Cajero inactivo.');
        END IF;

        IF v_cajero < p_monto THEN
            RAISE_APPLICATION_ERROR(-20206, 'Cajero sin fondos.');
        END IF;

        UPDATE cajero
        SET dinero_disponible_cajero = dinero_disponible_cajero - p_monto
        WHERE id_cajero = p_id_cajero;

        v_trx := fn_generar_id_transaccion;

        INSERT INTO transaccion VALUES (
            v_trx, p_id_cajero, p_id_tarjeta,
            NULL, NULL,
            SYSTIMESTAMP, 'cajero',
            p_monto, 'completada',
            'Retiro crédito'
        );

        o_id_transaccion := v_trx;
    END pr_retiro_credito;


    ------------------------------------------------------------------
    -- HISTORIAL CUENTA NORMAL
    ------------------------------------------------------------------
    PROCEDURE pr_historial_cuenta(
        p_id_cuenta IN NUMBER,
        o_cursor OUT SYS_REFCURSOR
    )
    IS
    BEGIN
        OPEN o_cursor FOR
            SELECT * FROM transaccion
            WHERE tipo_transaccion='cuenta'
            AND (id_cuenta_origen=p_id_cuenta
                OR id_cuenta_destino=p_id_cuenta)
            ORDER BY fecha_transaccion DESC;
    END;


    ------------------------------------------------------------------
    -- HISTORIAL DE CAJERO POR CUENTA
    ------------------------------------------------------------------
    PROCEDURE pr_historial_cajero(
        p_id_cuenta IN NUMBER,
        o_cursor OUT SYS_REFCURSOR
    )
    IS
    BEGIN
        OPEN o_cursor FOR
            SELECT t.* 
            FROM transaccion t
            JOIN tarjeta tar ON tar.id_tarjeta = t.id_tarjeta
            WHERE tipo_transaccion='cajero'
              AND tar.id_cuenta = p_id_cuenta
            ORDER BY fecha_transaccion DESC;
    END;

END pkgc_transacciones;
