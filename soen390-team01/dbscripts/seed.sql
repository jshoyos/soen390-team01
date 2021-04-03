CREATE FUNCTION public.timestamp_update()
    RETURNS trigger
    LANGUAGE 'plpgsql'
AS $$
BEGIN
    NEW.modified = now();
    return NEW;
END
$$;

ALTER FUNCTION public.timestamp_update()
    OWNER TO soen390team01devuser;

CREATE FUNCTION public.inventory_item_check() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN
	IF (
		(NEW.type = 'bike' AND EXISTS (select item_id from public.bike where item_id = NEW.item_id)) OR
		(NEW.type = 'part' AND EXISTS (select item_id from public.part where item_id = NEW.item_id)) OR
		(NEW.type = 'material' AND EXISTS (select item_id from public.material where item_id = NEW.item_id))
	   )
	   THEN RETURN NEW;
	ELSE
		RAISE EXCEPTION 'There is no existing % with item_id(%)', NEW.type, NEW.item_id;
	END IF;
END;
$$;


ALTER FUNCTION public.inventory_item_check() OWNER TO soen390team01devuser;

CREATE SEQUENCE public.bike_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.bike_id_seq OWNER TO soen390team01devuser;

SET default_tablespace = '';

SET default_table_access_method = heap;

CREATE TABLE public.bike (
    item_id bigint DEFAULT nextval('public.bike_id_seq'::regclass) NOT NULL,
    name character varying(64) NOT NULL,
    price money NOT NULL,
    grade character varying(32) NOT NULL,
    size character varying(4) NOT NULL,
    added timestamp without time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
    modified timestamp without time zone NOT NULL DEFAULT CURRENT_TIMESTAMP
);


ALTER TABLE public.bike OWNER TO soen390team01devuser;

CREATE TRIGGER bike_update_timestamp
    BEFORE UPDATE 
    ON public.bike
    FOR EACH ROW
    EXECUTE PROCEDURE public.timestamp_update();

CREATE TABLE public.bike_part (
    bike_id bigint NOT NULL,
    part_id bigint NOT NULL,
    part_quantity integer NOT NULL
);

ALTER TABLE public.bike_part OWNER TO soen390team01devuser;

CREATE TABLE public.inventory (
    item_id bigint NOT NULL,
    type character varying(8) NOT NULL,
    quantity integer NOT NULL,
    warehouse character varying(32) NOT NULL,
    inventory_id bigint NOT NULL,
    added timestamp without time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
    modified timestamp without time zone NOT NULL DEFAULT CURRENT_TIMESTAMP
);


ALTER TABLE public.inventory OWNER TO soen390team01devuser;

CREATE TRIGGER inventory_update_timestamp
    BEFORE UPDATE 
    ON public.inventory
    FOR EACH ROW
    EXECUTE PROCEDURE public.timestamp_update();

CREATE SEQUENCE public.inventory_inventory_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.inventory_inventory_id_seq OWNER TO soen390team01devuser;

ALTER SEQUENCE public.inventory_inventory_id_seq OWNED BY public.inventory.inventory_id;

CREATE SEQUENCE public.material_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.material_id_seq OWNER TO soen390team01devuser;

CREATE TABLE public.material (
    item_id bigint DEFAULT nextval('public.material_id_seq'::regclass) NOT NULL,
    name character varying(64) NOT NULL,
    price money NOT NULL,
    grade character varying(32) NOT NULL,
    added timestamp without time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
    modified timestamp without time zone NOT NULL DEFAULT CURRENT_TIMESTAMP
);


ALTER TABLE public.material OWNER TO soen390team01devuser;

CREATE TRIGGER material_timestamp_trigger
    BEFORE UPDATE 
    ON public.material
    FOR EACH ROW
    EXECUTE PROCEDURE public.timestamp_update();

CREATE SEQUENCE public.part_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.part_id_seq OWNER TO soen390team01devuser;

CREATE TABLE public.part (
    item_id bigint DEFAULT nextval('public.part_id_seq'::regclass) NOT NULL,
    name character varying(6) NOT NULL,
    price money NOT NULL,
    grade character varying(32) NOT NULL,
    size character varying(4) NOT NULL,
    added timestamp without time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
    modified timestamp without time zone NOT NULL DEFAULT CURRENT_TIMESTAMP
);


ALTER TABLE public.part OWNER TO soen390team01devuser;

CREATE TRIGGER part_timestamp_trigger
    BEFORE UPDATE 
    ON public.part
    FOR EACH ROW
    EXECUTE PROCEDURE public.timestamp_update();

CREATE TABLE public.part_material (
    part_id bigint NOT NULL,
    material_id bigint NOT NULL,
    material_quantity integer NOT NULL
);


ALTER TABLE public.part_material OWNER TO soen390team01devuser;

ALTER TABLE ONLY public.inventory ALTER COLUMN inventory_id SET DEFAULT nextval('public.inventory_inventory_id_seq'::regclass);

ALTER TABLE ONLY public.bike_part
    ADD CONSTRAINT bike_part_pkey PRIMARY KEY (bike_id, part_id);


ALTER TABLE ONLY public.bike
    ADD CONSTRAINT bike_pkey PRIMARY KEY (item_id);


ALTER TABLE ONLY public.inventory
    ADD CONSTRAINT inventory_item_id_type_key UNIQUE (item_id, type);


ALTER TABLE ONLY public.inventory
    ADD CONSTRAINT inventory_pkey PRIMARY KEY (inventory_id);


ALTER TABLE ONLY public.material
    ADD CONSTRAINT material_pkey PRIMARY KEY (item_id);


ALTER TABLE ONLY public.part_material
    ADD CONSTRAINT part_material_pkey PRIMARY KEY (part_id, material_id);


ALTER TABLE ONLY public.part
    ADD CONSTRAINT part_pkey PRIMARY KEY (item_id);


CREATE TRIGGER inventory_item_trigger
    BEFORE INSERT OR UPDATE OF item_id, type
    ON public.inventory
    FOR EACH ROW
    EXECUTE PROCEDURE public.inventory_item_check();


ALTER TABLE ONLY public.bike_part
    ADD CONSTRAINT bike_part_bike_id_fkey FOREIGN KEY (bike_id) REFERENCES public.bike(item_id) NOT VALID;


ALTER TABLE ONLY public.bike_part
    ADD CONSTRAINT bike_part_part_id_fkey FOREIGN KEY (part_id) REFERENCES public.part(item_id) NOT VALID;


ALTER TABLE ONLY public.part_material
    ADD CONSTRAINT part_material_material_id_fkey FOREIGN KEY (material_id) REFERENCES public.material(item_id) NOT VALID;

ALTER TABLE ONLY public.part_material
    ADD CONSTRAINT part_material_part_id_fkey FOREIGN KEY (part_id) REFERENCES public.part(item_id) NOT VALID;

CREATE SEQUENCE public.customer_customer_id_seq
    INCREMENT 1
    START 1
    MINVALUE 1
    MAXVALUE 9223372036854775807
    CACHE 1;

ALTER SEQUENCE public.customer_customer_id_seq
    OWNER TO soen390team01devuser;

CREATE SEQUENCE public.order_order_id_seq
    INCREMENT 1
    START 1
    MINVALUE 1
    MAXVALUE 9223372036854775807
    CACHE 1;

ALTER SEQUENCE public.order_order_id_seq
    OWNER TO soen390team01devuser;

CREATE SEQUENCE public.payment_payment_id_seq
    INCREMENT 1
    START 1
    MINVALUE 1
    MAXVALUE 9223372036854775807
    CACHE 1;

ALTER SEQUENCE public.payment_payment_id_seq
    OWNER TO soen390team01devuser;

CREATE SEQUENCE public.procurement_procurement_id_seq
    INCREMENT 1
    START 1
    MINVALUE 1
    MAXVALUE 9223372036854775807
    CACHE 1;

ALTER SEQUENCE public.procurement_procurement_id_seq
    OWNER TO soen390team01devuser;

CREATE SEQUENCE public.vendor_vendor_id_seq
    INCREMENT 1
    START 1
    MINVALUE 1
    MAXVALUE 9223372036854775807
    CACHE 1;

ALTER SEQUENCE public.vendor_vendor_id_seq
    OWNER TO soen390team01devuser;

CREATE TABLE public.customer
(
    customer_id bigint NOT NULL DEFAULT nextval('customer_customer_id_seq'::regclass),
    name character varying(32) COLLATE pg_catalog."default" NOT NULL,
    address character varying(32) COLLATE pg_catalog."default" NOT NULL,
    phone_number character varying(10) COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT customer_pkey PRIMARY KEY (customer_id)
)

TABLESPACE pg_default;

ALTER TABLE public.customer
    OWNER to soen390team01devuser;

CREATE TABLE public.vendor
(
    vendor_id bigint NOT NULL DEFAULT nextval('vendor_vendor_id_seq'::regclass),
    name character varying(32) COLLATE pg_catalog."default" NOT NULL,
    address character varying(32) COLLATE pg_catalog."default" NOT NULL,
    phone_number character varying(10) COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT vendor_pkey PRIMARY KEY (vendor_id)
)

TABLESPACE pg_default;

ALTER TABLE public.vendor
    OWNER to soen390team01devuser;

CREATE TABLE public.payment
(
    payment_id bigint NOT NULL DEFAULT nextval('payment_payment_id_seq'::regclass),
    amount money NOT NULL,
    state character varying(10) COLLATE pg_catalog."default" NOT NULL,
    added timestamp without time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
    modified timestamp without time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT payment_pkey PRIMARY KEY (payment_id)
)

TABLESPACE pg_default;

ALTER TABLE public.payment
    OWNER to soen390team01devuser;

CREATE TRIGGER payment_timestamp_trigger
    BEFORE UPDATE 
    ON public.payment
    FOR EACH ROW
    EXECUTE PROCEDURE public.timestamp_update();

CREATE TABLE public."order"
(
    order_id bigint NOT NULL DEFAULT nextval('order_order_id_seq'::regclass),
    customer_id bigint NOT NULL,
    state character varying(10) COLLATE pg_catalog."default" NOT NULL,
    payment_id bigint NOT NULL,
    added timestamp without time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
    modified timestamp without time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT order_pkey PRIMARY KEY (order_id),
    CONSTRAINT order_customer_id_fkey FOREIGN KEY (customer_id)
        REFERENCES public.customer (customer_id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
        NOT VALID,
    CONSTRAINT order_payment_id_fkey FOREIGN KEY (payment_id)
        REFERENCES public.payment (payment_id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
        NOT VALID
)

TABLESPACE pg_default;

ALTER TABLE public."order"
    OWNER to soen390team01devuser;

CREATE TRIGGER order_timestamp_trigger
    BEFORE UPDATE 
    ON public."order"
    FOR EACH ROW
    EXECUTE PROCEDURE public.timestamp_update();

CREATE TABLE public.procurement
(
    procurement_id bigint NOT NULL DEFAULT nextval('procurement_procurement_id_seq'::regclass),
    item_id bigint NOT NULL,
    payment_id bigint NOT NULL,
    item_quantity integer NOT NULL,
    state character varying(10) COLLATE pg_catalog."default" NOT NULL,
    type character varying(8) COLLATE pg_catalog."default" NOT NULL,
    vendor_id bigint NOT NULL,
    added timestamp without time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
    modified timestamp without time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT procurement_pkey PRIMARY KEY (procurement_id),
    CONSTRAINT procurement_payment_id_fkey FOREIGN KEY (payment_id)
        REFERENCES public.payment (payment_id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
        NOT VALID,
    CONSTRAINT procurement_vendor_id_fkey FOREIGN KEY (vendor_id)
        REFERENCES public.vendor (vendor_id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
        NOT VALID
)

TABLESPACE pg_default;

ALTER TABLE public.procurement
    OWNER to soen390team01devuser;

CREATE TRIGGER procurement_timestamp_trigger
    BEFORE UPDATE 
    ON public.procurement
    FOR EACH ROW
    EXECUTE PROCEDURE public.timestamp_update();

CREATE TRIGGER procurement_item_trigger
    BEFORE INSERT OR UPDATE 
    ON public.procurement
    FOR EACH ROW
    EXECUTE PROCEDURE public.inventory_item_check();

CREATE TABLE public.order_item
(
    order_id bigint NOT NULL,
    item_id bigint NOT NULL,
    item_quantity integer NOT NULL,
    type character varying(8) COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT order_item_pkey PRIMARY KEY (type, order_id, item_id),
    CONSTRAINT order_item_order_id_fkey FOREIGN KEY (order_id)
        REFERENCES public."order" (order_id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
        NOT VALID
)

TABLESPACE pg_default;

ALTER TABLE public.order_item
    OWNER to soen390team01devuser;

CREATE TRIGGER order_item_trigger
    BEFORE INSERT OR UPDATE 
    ON public.order_item
    FOR EACH ROW
    EXECUTE PROCEDURE public.inventory_item_check();

CREATE SEQUENCE public.user_user_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;

CREATE TABLE public."user"
(
    user_role character varying COLLATE pg_catalog."default" NOT NULL,
    phone_number character varying COLLATE pg_catalog."default" NOT NULL,
    last_name character varying COLLATE pg_catalog."default" NOT NULL,
    first_name character varying COLLATE pg_catalog."default" NOT NULL,
    email character varying COLLATE pg_catalog."default" NOT NULL,
    user_id bigint NOT NULL DEFAULT nextval('user_user_id_seq'::regclass),
    iv character varying COLLATE pg_catalog."default" NOT NULL,
    added timestamp without time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
    modified timestamp without time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT user_pkey PRIMARY KEY (user_id),
    CONSTRAINT email_unique UNIQUE (email)
)

TABLESPACE pg_default;

ALTER TABLE public."user"
    OWNER to soen390team01devuser;


CREATE TRIGGER user_timestamp_trigger
    BEFORE UPDATE 
    ON public."user"
    FOR EACH ROW
    EXECUTE PROCEDURE public.timestamp_update();

CREATE SEQUENCE public.production_production_id_seq
    INCREMENT 1
    START 1
    MINVALUE 1
    MAXVALUE 9223372036854775807
    CACHE 1;

ALTER SEQUENCE public.production_production_id_seq
    OWNER TO soen390team01devuser;

CREATE TABLE public.production
(
    state character varying(10) COLLATE pg_catalog."default" NOT NULL,
    added timestamp without time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
    modified timestamp without time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
    bike_id bigint NOT NULL,
    production_id bigint NOT NULL DEFAULT nextval('production_production_id_seq'::regclass),
    quantity integer NOT NULL,
    CONSTRAINT production_pkey PRIMARY KEY (production_id),
    CONSTRAINT product_bike_id_fkey FOREIGN KEY (bike_id)
        REFERENCES public.bike (item_id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
        NOT VALID
)

TABLESPACE pg_default;

ALTER TABLE public.production
    OWNER to soen390team01devuser;


CREATE TRIGGER production_update_timestamp
    BEFORE UPDATE 
    ON public.production
    FOR EACH ROW
    EXECUTE PROCEDURE public.timestamp_update();


INSERT INTO public."user"(
	user_role, phone_number, last_name, first_name, email, iv)
	VALUES ('Admin', 'RIMzfjzV+VcTL2/DXk/2QA==',
	'aaFRyoeTmIB970WmVA5H9g==',
	'aaFRyoeTmIB970WmVA5H9g==',
	'23jT32dBjzfnrf39mHW0X0/QL0ZV5EZ+XucorndBCks=',
	'T4qPYRHF2EQDIHJF1fAsRQ==');

INSERT INTO public.bike(item_id, name, price, grade, size) VALUES (1,'bike1','500','Gold','M');
INSERT INTO public.bike(item_id, name, price, grade, size) VALUES (2,'bike2','600','Aluminum','M');
INSERT INTO public.bike(item_id, name, price, grade, size) VALUES (3,'bike3','500','Aluminum','L');
INSERT INTO public.bike(item_id, name, price, grade, size) VALUES (4,'bike4','400','Gold','S');
INSERT INTO public.bike(item_id, name, price, grade, size) VALUES (5,'bike5','200','Copper','L');

INSERT INTO public.material(item_id,name, price, grade) VALUES (6,'Mat1', '100', 'Gold');
INSERT INTO public.material(item_id,name, price, grade) VALUES (7,'Mat2', '50', 'Copper');
INSERT INTO public.material(item_id,name, price, grade) VALUES (8,'Mat3', '40', 'Copper');
INSERT INTO public.material(item_id,name, price, grade) VALUES (9,'Mat4', '30', 'Silver');
INSERT INTO public.material(item_id,name, price, grade) VALUES (10,'Mat5', '10', 'Iron');
INSERT INTO public.material(item_id,name, price, grade) VALUES (11,'Mat6', '67', 'Aluminum');
INSERT INTO public.material(item_id,name, price, grade) VALUES (12,'Mat7', '24', 'Copper');
INSERT INTO public.material(item_id,name, price, grade) VALUES (13,'Mat8', '20', 'Silver');

INSERT INTO public.part(item_id, name, price, grade, size) VALUES (14,'saddle', '20', 'Aluminum', 'L');
INSERT INTO public.part(item_id, name, price, grade, size) VALUES (15,'brake', '18', 'Aluminum', 'M');
INSERT INTO public.part(item_id, name, price, grade, size) VALUES (16,'chain', '16', 'Iron', 'S');
INSERT INTO public.part(item_id, name, price, grade, size) VALUES (17,'cogset', '20', 'Copper', 'L');
INSERT INTO public.part(item_id, name, price, grade, size) VALUES (18,'wheel', '18', 'Aluminum', 'M');
INSERT INTO public.part(item_id, name, price, grade, size) VALUES (19,'pedal', '16', 'Copper', 'S');
INSERT INTO public.part(item_id, name, price, grade, size) VALUES (20,'handle', '20', 'Rubber', 'L');
INSERT INTO public.part(item_id, name, price, grade, size) VALUES (21,'fork', '18', 'Aluminum', 'M');
INSERT INTO public.part(item_id, name, price, grade, size) VALUES (22,'tube', '16', 'Aluminum', 'S');

INSERT INTO public.bike_part(bike_id, part_id, part_quantity)VALUES (1, 14, 1);
INSERT INTO public.bike_part(bike_id, part_id, part_quantity)VALUES (1, 15, 2);
INSERT INTO public.bike_part(bike_id, part_id, part_quantity)VALUES (1, 16, 2);
INSERT INTO public.bike_part(bike_id, part_id, part_quantity)VALUES (1, 17, 4);
INSERT INTO public.bike_part(bike_id, part_id, part_quantity)VALUES (1, 18, 2);
INSERT INTO public.bike_part(bike_id, part_id, part_quantity)VALUES (1, 19, 2);
INSERT INTO public.bike_part(bike_id, part_id, part_quantity)VALUES (1, 20, 2);
INSERT INTO public.bike_part(bike_id, part_id, part_quantity)VALUES (1, 21, 1);
INSERT INTO public.bike_part(bike_id, part_id, part_quantity)VALUES (1, 22, 2);
INSERT INTO public.bike_part(bike_id, part_id, part_quantity)VALUES (2, 14, 2);
INSERT INTO public.bike_part(bike_id, part_id, part_quantity)VALUES (2, 15, 1);
INSERT INTO public.bike_part(bike_id, part_id, part_quantity)VALUES (2, 16, 1);
INSERT INTO public.bike_part(bike_id, part_id, part_quantity)VALUES (2, 19, 2);
INSERT INTO public.bike_part(bike_id, part_id, part_quantity)VALUES (2, 18, 1);
INSERT INTO public.bike_part(bike_id, part_id, part_quantity)VALUES (3, 14, 2);
INSERT INTO public.bike_part(bike_id, part_id, part_quantity)VALUES (3, 15, 1);
INSERT INTO public.bike_part(bike_id, part_id, part_quantity)VALUES (3, 16, 1);

INSERT INTO public.part_material(part_id, material_id, material_quantity)VALUES (14, 6, 5);
INSERT INTO public.part_material(part_id, material_id, material_quantity)VALUES (14, 7, 6);
INSERT INTO public.part_material(part_id, material_id, material_quantity)VALUES (14, 8, 2);
INSERT INTO public.part_material(part_id, material_id, material_quantity)VALUES (15, 9, 2);
INSERT INTO public.part_material(part_id, material_id, material_quantity)VALUES (15, 10, 2);
INSERT INTO public.part_material(part_id, material_id, material_quantity)VALUES (16, 6, 3);
INSERT INTO public.part_material(part_id, material_id, material_quantity)VALUES (16, 7, 2);
INSERT INTO public.part_material(part_id, material_id, material_quantity)VALUES (17, 6, 1);
INSERT INTO public.part_material(part_id, material_id, material_quantity)VALUES (17, 7, 1);
INSERT INTO public.part_material(part_id, material_id, material_quantity)VALUES (17, 8, 1);
INSERT INTO public.part_material(part_id, material_id, material_quantity)VALUES (17, 9, 1);
INSERT INTO public.part_material(part_id, material_id, material_quantity)VALUES (18, 12, 4);
INSERT INTO public.part_material(part_id, material_id, material_quantity)VALUES (19, 13, 4);
INSERT INTO public.part_material(part_id, material_id, material_quantity)VALUES (20, 6, 3);
INSERT INTO public.part_material(part_id, material_id, material_quantity)VALUES (20, 9, 2);
INSERT INTO public.part_material(part_id, material_id, material_quantity)VALUES (21, 11, 3);
INSERT INTO public.part_material(part_id, material_id, material_quantity)VALUES (22, 10, 2);

INSERT INTO public.inventory(item_id, type, quantity, warehouse) VALUES ('1', 'bike', '5', 'Warehouse1');
INSERT INTO public.inventory(item_id, type, quantity, warehouse) VALUES ('2', 'bike', '6', 'Warehouse1');
INSERT INTO public.inventory(item_id, type, quantity, warehouse) VALUES ('3', 'bike', '7', 'Warehouse1');
INSERT INTO public.inventory(item_id, type, quantity, warehouse) VALUES ('4', 'bike', '8', 'Warehouse2');
INSERT INTO public.inventory(item_id, type, quantity, warehouse) VALUES ('5', 'bike', '9', 'Warehouse2');
INSERT INTO public.inventory(item_id, type, quantity, warehouse) VALUES ('6', 'material', '8', 'Warehouse1');
INSERT INTO public.inventory(item_id, type, quantity, warehouse) VALUES ('7', 'material', '14', 'Warehouse1');
INSERT INTO public.inventory(item_id, type, quantity, warehouse) VALUES ('8', 'material', '20', 'Warehouse1');
INSERT INTO public.inventory(item_id, type, quantity, warehouse) VALUES ('9', 'material', '30', 'Warehouse2');
INSERT INTO public.inventory(item_id, type, quantity, warehouse) VALUES ('10', 'material', '23', 'Warehouse1');
INSERT INTO public.inventory(item_id, type, quantity, warehouse) VALUES ('11', 'material', '20', 'Warehouse1');
INSERT INTO public.inventory(item_id, type, quantity, warehouse) VALUES ('12', 'material', '10', 'Warehouse1');
INSERT INTO public.inventory(item_id, type, quantity, warehouse) VALUES ('13', 'material', '12', 'Warehouse2');
INSERT INTO public.inventory(item_id, type, quantity, warehouse) VALUES ('14', 'part', '11', 'Warehouse2');
INSERT INTO public.inventory(item_id, type, quantity, warehouse) VALUES ('15', 'part', '24', 'Warehouse1');
INSERT INTO public.inventory(item_id, type, quantity, warehouse) VALUES ('16', 'part', '25', 'Warehouse2');
INSERT INTO public.inventory(item_id, type, quantity, warehouse) VALUES ('17', 'part', '15', 'Warehouse2');
INSERT INTO public.inventory(item_id, type, quantity, warehouse) VALUES ('18', 'part', '20', 'Warehouse1');
INSERT INTO public.inventory(item_id, type, quantity, warehouse) VALUES ('19', 'part', '30', 'Warehouse2');
INSERT INTO public.inventory(item_id, type, quantity, warehouse) VALUES ('20', 'part', '10', 'Warehouse2');
INSERT INTO public.inventory(item_id, type, quantity, warehouse) VALUES ('21', 'part', '19', 'Warehouse1');
INSERT INTO public.inventory(item_id, type, quantity, warehouse) VALUES ('22', 'part', '20', 'Warehouse2');


INSERT INTO public.payment(payment_id, amount, state) VALUES (1, 100, 'pending');
INSERT INTO public.payment(payment_id, amount, state) VALUES (2, -200, 'canceled');
INSERT INTO public.payment(payment_id, amount, state) VALUES (3, -300, 'pending');
INSERT INTO public.payment(payment_id, amount, state) VALUES (4, 400, 'completed');
INSERT INTO public.payment(payment_id, amount, state) VALUES (5, 500, 'completed');
INSERT INTO public.payment(payment_id, amount, state) VALUES (6, -300, 'completed');
INSERT INTO public.payment(payment_id, amount, state) VALUES (7, -100, 'canceled');
INSERT INTO public.payment(payment_id, amount, state) VALUES (8, -500, 'completed');

INSERT INTO public.vendor(vendor_id, name, address, phone_number)VALUES (1, 'vendor1', 'address1', '5140000001');
INSERT INTO public.vendor(vendor_id, name, address, phone_number)VALUES (2, 'vendor2', 'address2', '5140000002');
INSERT INTO public.vendor(vendor_id, name, address, phone_number)VALUES (3, 'vendor3', 'address3', '5140000003');
INSERT INTO public.vendor(vendor_id, name, address, phone_number)VALUES (4, 'vendor4', 'address4', '5140000004');
INSERT INTO public.vendor(vendor_id, name, address, phone_number)VALUES (5, 'vendor5', 'address5', '5140000005');

INSERT INTO public.customer(customer_id, name, address, phone_number) VALUES (1, 'customer1', 'address1', '514000001');
INSERT INTO public.customer(customer_id, name, address, phone_number) VALUES (2, 'customer2', 'address2', '514000002');
INSERT INTO public.customer(customer_id, name, address, phone_number) VALUES (3, 'customer3', 'address3', '514000003');
INSERT INTO public.customer(customer_id, name, address, phone_number) VALUES (4, 'customer4', 'address4', '514000004');
INSERT INTO public.customer(customer_id, name, address, phone_number) VALUES (5, 'customer5', 'address5', '514000005');

INSERT INTO public."order"(customer_id, state, payment_id) VALUES (1, 'pending' , 1);
INSERT INTO public."order"(customer_id, state, payment_id) VALUES (2, 'pending' , 3);
INSERT INTO public."order"(customer_id, state, payment_id) VALUES (3, 'canceled' , 2);
INSERT INTO public."order"(customer_id, state, payment_id) VALUES (4, 'completed' , 4);
INSERT INTO public."order"(customer_id, state, payment_id) VALUES (5, 'completed' , 5);

INSERT INTO public.procurement(item_id, payment_id, item_quantity, state, type, vendor_id) VALUES (1, 1, 1, 'pending', 'bike', 1);
INSERT INTO public.procurement(item_id, payment_id, item_quantity, state, type, vendor_id) VALUES (2, 3, 1, 'pending', 'bike', 2);
INSERT INTO public.procurement(item_id, payment_id, item_quantity, state, type, vendor_id) VALUES (6, 4, 1, 'completed', 'material', 3);
INSERT INTO public.procurement(item_id, payment_id, item_quantity, state, type, vendor_id) VALUES (7, 5, 1, 'completed', 'material', 4);
INSERT INTO public.procurement(item_id, payment_id, item_quantity, state, type, vendor_id) VALUES (8, 2, 1, 'canceled', 'material', 5);
INSERT INTO public.procurement(item_id, payment_id, item_quantity, state, type, vendor_id) VALUES (14, 3, 1, 'pending', 'part', 5);
