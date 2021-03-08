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
    size character varying(4) NOT NULL
);


ALTER TABLE public.bike OWNER TO soen390team01devuser;


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
    inventory_id bigint NOT NULL
);


ALTER TABLE public.inventory OWNER TO soen390team01devuser;

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
    grade character varying(32) NOT NULL
);


ALTER TABLE public.material OWNER TO soen390team01devuser;

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
    size character varying(4) NOT NULL
);


ALTER TABLE public.part OWNER TO soen390team01devuser;

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
    CONSTRAINT payment_pkey PRIMARY KEY (payment_id)
)

TABLESPACE pg_default;

ALTER TABLE public.payment
    OWNER to soen390team01devuser;

CREATE TABLE public."order"
(
    order_id bigint NOT NULL DEFAULT nextval('order_order_id_seq'::regclass),
    customer_id bigint NOT NULL,
    state character varying(10) COLLATE pg_catalog."default" NOT NULL,
    payment_id bigint NOT NULL,
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

CREATE TABLE public.procurement
(
    procurement_id bigint NOT NULL DEFAULT nextval('procurement_procurement_id_seq'::regclass),
    item_id bigint NOT NULL,
    payment_id bigint NOT NULL,
    item_quantity integer NOT NULL,
    state character varying(10) COLLATE pg_catalog."default" NOT NULL,
    type character varying(8) COLLATE pg_catalog."default" NOT NULL,
    vendor_id bigint NOT NULL,
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
    CONSTRAINT user_pkey PRIMARY KEY (user_id),
    CONSTRAINT email_unique UNIQUE (email)
)

TABLESPACE pg_default;

ALTER TABLE public."user"
    OWNER to soen390team01devuser;

INSERT INTO public."user"(
	user_role, phone_number, last_name, first_name, email, iv)
	VALUES ('Admin', 'RIMzfjzV+VcTL2/DXk/2QA==',
	'aaFRyoeTmIB970WmVA5H9g==',
	'aaFRyoeTmIB970WmVA5H9g==',
	'23jT32dBjzfnrf39mHW0X0/QL0ZV5EZ+XucorndBCks=',
	'T4qPYRHF2EQDIHJF1fAsRQ==');
