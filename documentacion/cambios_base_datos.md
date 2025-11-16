# 📋 CAMBIOS EN BASE DE DATOS - Versión Actual vs Versión Anterior

**Fecha de actualización:** 16 de Noviembre de 2025

Este documento detalla todos los cambios realizados en la base de datos comparando la versión anterior con la versión actual.

**⚠️ NOTA IMPORTANTE:** La base de datos antigua fue exportada con el prefijo `BANCO.` en los nombres de tablas. La versión actual NO usa este prefijo.

---

## 🔍 1. DIFERENCIA EN NOMBRES DE TABLAS

### **Versión Anterior (Exportada):**
Las tablas tienen el prefijo `BANCO.`:
- `BANCO.CLIENTE`
- `BANCO.CUENTA`
- `BANCO.ADMINISTRADOR`
- etc.

### **Versión Actual:**
Las tablas NO tienen prefijo:
- `CLIENTE`
- `CUENTA`
- `ADMINISTRADOR`
- etc.

**⚠️ Si estás migrando desde la versión anterior, asegúrate de eliminar el prefijo `BANCO.` de todas las referencias en los procedimientos almacenados.**

---

## 🔧 2. CORRECCIÓN: CONSTRAINT DE CUOTA (MONTO vs CAPITAL)

### **Problema Detectado:**
El constraint original exigía que `MONTO_DE_CUOTA = CAPITAL_CUOTA`, pero en préstamos con interés, el monto incluye interés y el capital es solo la parte del préstamo.

### **Solución:**

```sql
-- ============================================================
-- PASO 1: Eliminar constraint antiguo si existe
-- ============================================================
BEGIN
    EXECUTE IMMEDIATE 'ALTER TABLE CUOTA DROP CONSTRAINT CK_CUOTA_CONSIST';
EXCEPTION
    WHEN OTHERS THEN
        IF SQLCODE != -2443 THEN  -- -2443 = constraint no existe
            RAISE;
        END IF;
END;
/

-- ============================================================
-- PASO 2: Eliminar constraint nuevo si ya existe (por si se ejecutó antes)
-- ============================================================
BEGIN
    EXECUTE IMMEDIATE 'ALTER TABLE CUOTA DROP CONSTRAINT CK_CUOTA_MONTO_CAPITAL';
EXCEPTION
    WHEN OTHERS THEN
        IF SQLCODE != -2443 THEN
            RAISE;
        END IF;
END;
/

-- ============================================================
-- PASO 3: Crear nuevo constraint que permite capital <= monto
-- ============================================================
ALTER TABLE CUOTA ADD CONSTRAINT CK_CUOTA_MONTO_CAPITAL 
    CHECK (MONTO_DE_CUOTA >= CAPITAL_CUOTA AND CAPITAL_CUOTA >= 0);
```

**Explicación:**
- El nuevo constraint permite que `MONTO_DE_CUOTA >= CAPITAL_CUOTA`
- Esto permite que el monto incluya interés mientras el capital es solo la parte del préstamo
- Ambos valores deben ser >= 0

---

## 📅 3. CORRECCIÓN: GENERACIÓN DE FECHAS DE VENCIMIENTO EN CUOTAS

### **Problema Detectado:**
El procedimiento `pkgc_cuotas.pr_generar_cuotas` sumaba **días** en lugar de **meses** para las fechas de vencimiento.

### **Solución:**

```sql
-- ============================================================
-- CORREGIR pkgc_cuotas.pr_generar_cuotas
-- ============================================================
CREATE OR REPLACE PACKAGE BODY pkgc_cuotas AS

    FUNCTION fn_generar_id_cuota RETURN NUMBER IS
        v_id NUMBER;
    BEGIN
        SELECT NVL(MAX(id_cuota),0) + 1
        INTO v_id
        FROM cuota;
        RETURN v_id;
    END fn_generar_id_cuota;

    ----------------------------------------------------------------------
    -- Generar cuotas de un préstamo activo (CORREGIDO)
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
        -- Traer datos del préstamo
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
                ADD_MONTHS(v_fecha_ini, v_i),  -- ✅ CORREGIDO: Suma MESES, no días
                NULL,
                'pendiente'
            );
        END LOOP;

    EXCEPTION
        WHEN NO_DATA_FOUND THEN
            DBMS_OUTPUT.PUT_LINE('Aviso: préstamo '||p_id_prestamo||' no existe al generar cuotas.');
        WHEN OTHERS THEN
            RAISE;
    END pr_generar_cuotas;

    -- ... (resto del package body) ...
END pkgc_cuotas;
/
COMMIT;
```

**Cambio Clave:**
- **Antes:** `v_fecha_ini + v_i` (sumaba días)
- **Ahora:** `ADD_MONTHS(v_fecha_ini, v_i)` (suma meses)

---

## 🔐 4. CORRECCIÓN: ERROR TIPOGRÁFICO EN pkg_administrador

### **Problema Detectado:**
El procedimiento `fn_login_admin` tenía un error tipográfico en el nombre de columna.

### **Solución:**

```sql
-- ============================================================
-- CORREGIR pkg_administrador.fn_login_admin
-- ============================================================
CREATE OR REPLACE PACKAGE BODY pkg_administrador AS

  FUNCTION fn_login_admin(
    p_email       IN VARCHAR2,
    p_contrasena  IN VARCHAR2
  ) RETURN NUMBER IS
    v_id NUMBER;
  BEGIN
    -- ✅ CORREGIDO: id_administrador (antes decía id_adminISTRADOR)
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
/
COMMIT;
```

---

## 💰 5. VERIFICACIÓN: PROCEDIMIENTOS DE CUENTA

### **Estado Actual:**

Los procedimientos de cuenta están correctos y funcionando como se espera:

#### **1. pkg_cuenta.actualizar_cuenta:**
- **Comportamiento:** REEMPLAZA el saldo con el valor enviado en `p_saldo`
- **Uso:** `UPDATE cuenta SET saldo_cuenta = p_saldo`
- **✅ CORRECTO:** Este procedimiento debe reemplazar, no sumar

#### **2. pkg_cuenta.cambiar_saldo:**
- **Comportamiento:** SUMA el monto al saldo actual
- **Uso:** `UPDATE cuenta SET saldo_cuenta = saldo_actual + p_monto`
- **✅ CORRECTO:** Este procedimiento suma correctamente

**⚠️ NOTA IMPORTANTE:**
- La lógica de suma para `cambiar_saldo` fue movida al servicio (service_Cuenta.cs) en la API
- El procedimiento `cambiar_saldo` en la BD sigue funcionando correctamente
- Actualmente el servicio usa `actualizar_cuenta` después de calcular la suma en C#

**La base de datos NO requiere cambios para esta funcionalidad.**

---

## 🛠️ 6. CORRECCIÓN: Manejo de Excepciones en cambiar_saldo

### **Problema Detectado:**
El procedimiento `pkg_cuenta.cambiar_saldo` tenía un manejo incorrecto de la excepción `NO_DATA_FOUND`. Cuando la cuenta no existe, solo imprime un mensaje pero no lanza un error.

### **Solución:**

```sql
-- ============================================================
-- CORREGIR pkg_cuenta.cambiar_saldo
-- ============================================================
CREATE OR REPLACE PACKAGE BODY pkg_cuenta AS
    -- ... (otros procedimientos y funciones) ...

    -------------------------------------------------------------------
    -- Procedimiento: cambiar_saldo (CORREGIDO)
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

    -- ... (resto del package body) ...
END pkg_cuenta;
/
COMMIT;
```

**Cambios:**
- **Antes:** Solo imprimía mensaje con `DBMS_OUTPUT.PUT_LINE` cuando no encontraba la cuenta
- **Ahora:** Lanza `RAISE_APPLICATION_ERROR(-20008, 'La cuenta no existe.')` para que la API pueda capturar el error

---

## 📋 RESUMEN DE CAMBIOS

| # | Cambio | Tipo | Estado |
|---|--------|------|--------|
| 1 | Constraint de CUOTA (MONTO vs CAPITAL) | Corrección | ✅ Aplicado |
| 2 | Fechas de vencimiento en cuotas (meses vs días) | Corrección | ✅ Aplicado |
| 3 | Error tipográfico en pkg_administrador | Corrección | ✅ Aplicado |
| 4 | Manejo de excepciones en cambiar_saldo | Corrección | ✅ Aplicado |
| 5 | Verificación de procedimientos de cuenta | Verificación | ✅ Correcto |

---

## 🚀 SCRIPT COMPLETO DE MIGRACIÓN

Si necesitas aplicar todos los cambios de una vez, ejecuta el siguiente script:

```sql
-- ============================================================
-- SCRIPT COMPLETO DE MIGRACIÓN DE BASE DE DATOS
-- Ejecutar en Oracle SQL Developer
-- ============================================================

-- 1. CORREGIR CONSTRAINT DE CUOTA
BEGIN
    EXECUTE IMMEDIATE 'ALTER TABLE CUOTA DROP CONSTRAINT CK_CUOTA_CONSIST';
EXCEPTION
    WHEN OTHERS THEN
        IF SQLCODE != -2443 THEN
            RAISE;
        END IF;
END;
/

BEGIN
    EXECUTE IMMEDIATE 'ALTER TABLE CUOTA DROP CONSTRAINT CK_CUOTA_MONTO_CAPITAL';
EXCEPTION
    WHEN OTHERS THEN
        IF SQLCODE != -2443 THEN
            RAISE;
        END IF;
END;
/

ALTER TABLE CUOTA ADD CONSTRAINT CK_CUOTA_MONTO_CAPITAL 
    CHECK (MONTO_DE_CUOTA >= CAPITAL_CUOTA AND CAPITAL_CUOTA >= 0);

-- ============================================================
-- 2. CORREGIR pkgc_cuotas.pr_generar_cuotas
-- ============================================================
-- El problema está en pkgc_cuotas.pr_generar_cuotas
-- Actualmente hace: v_fecha_ini + v_i (suma días)
-- Debería sumar MESES, no días
-- ============================================================
CREATE OR REPLACE PACKAGE BODY pkgc_cuotas AS

    FUNCTION fn_generar_id_cuota RETURN NUMBER IS
        v_id NUMBER;
    BEGIN
        SELECT NVL(MAX(id_cuota),0) + 1
        INTO v_id
        FROM cuota;
        RETURN v_id;
    END fn_generar_id_cuota;

    ----------------------------------------------------------------------
    -- Generar cuotas de un préstamo activo (CORREGIDO)
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
                v_valor_cuota,  -- Por ahora capital = monto, pero puede cambiar si hay interés
                ADD_MONTHS(v_fecha_ini, v_i),  -- ✅ CORREGIDO: Suma MESES, no días
                NULL,
                'pendiente'
            );
        END LOOP;

    EXCEPTION
        WHEN NO_DATA_FOUND THEN
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
/

-- ============================================================
-- 3. CORREGIR pkg_administrador.fn_login_admin
-- ============================================================
-- Error tipográfico en el nombre de columna
-- ============================================================
CREATE OR REPLACE PACKAGE BODY pkg_administrador AS

  FUNCTION fn_login_admin(
    p_email       IN VARCHAR2,
    p_contrasena  IN VARCHAR2
  ) RETURN NUMBER IS
    v_id NUMBER;
  BEGIN
    -- ✅ CORREGIDO: id_administrador (antes decía id_adminISTRADOR)
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
/

-- ============================================================
-- 4. CORREGIR pkg_cuenta.cambiar_saldo
-- ============================================================
-- Manejo incorrecto de excepciones: ahora lanza error cuando
-- la cuenta no existe en lugar de solo imprimir mensaje
-- ============================================================
CREATE OR REPLACE PACKAGE BODY pkg_cuenta AS
    -- ... (otros procedimientos y funciones del package) ...

    -------------------------------------------------------------------
    -- Procedimiento: cambiar_saldo (CORREGIDO)
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

    -- ... (resto del package body) ...
END pkg_cuenta;
/

COMMIT;
```

---

## ⚠️ NOTAS IMPORTANTES PARA MIGRACIÓN

1. **Prefijo BANCO.:** Si tu base de datos antigua tiene el prefijo `BANCO.`, asegúrate de:
   - Eliminar el prefijo de todas las referencias en procedimientos almacenados
   - O actualizar los procedimientos para usar el prefijo correcto

2. **Backup:** Siempre haz un backup antes de ejecutar scripts de migración

3. **Orden de Ejecución:** Ejecuta los scripts en el orden indicado

4. **Testing:** Después de aplicar los cambios, prueba todos los procedimientos almacenados

5. **Triggers:** Los triggers para generar IDs automáticamente siguen siendo los mismos (ver `scriptbase/crear_triggers_ids.sql`)

---

## 📝 COMANDOS ÚTILES

### Verificar constraints de una tabla:
```sql
SELECT constraint_name, constraint_type, search_condition
FROM user_constraints
WHERE table_name = 'CUOTA';
```

### Verificar procedimientos almacenados:
```sql
SELECT object_name, object_type, status
FROM user_objects
WHERE object_type IN ('PROCEDURE', 'FUNCTION', 'PACKAGE', 'PACKAGE BODY')
ORDER BY object_type, object_name;
```

### Verificar errores de compilación:
```sql
SELECT name, type, line, text
FROM user_errors
WHERE name = 'PKG_CUENTA'
ORDER BY sequence;
```

---

**Última actualización:** 16 de Noviembre de 2025

**Archivo de referencia:** `scriptbase/correcciones_base_datos.sql`

