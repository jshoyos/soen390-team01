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


ALTER TABLE ONLY public.bike_part
    ADD CONSTRAINT bike_part_bike_id_fkey FOREIGN KEY (bike_id) REFERENCES public.bike(item_id) NOT VALID;



ALTER TABLE ONLY public.bike_part
    ADD CONSTRAINT bike_part_part_id_fkey FOREIGN KEY (part_id) REFERENCES public.part(item_id) NOT VALID;



ALTER TABLE ONLY public.part_material
    ADD CONSTRAINT part_material_material_id_fkey FOREIGN KEY (material_id) REFERENCES public.material(item_id) NOT VALID;


ALTER TABLE ONLY public.part_material
    ADD CONSTRAINT part_material_part_id_fkey FOREIGN KEY (part_id) REFERENCES public.part(item_id) NOT VALID;
