CREATE TABLE IF NOT EXISTS public.clientes (
    id SERIAL PRIMARY KEY,
    limite INT NOT NULL,
    saldo INT NOT NULL
);

CREATE TABLE IF NOT EXISTS public.transacoes (
    id SERIAL PRIMARY KEY,
    valor INT NOT NULL,
    tipo CHAR NOT NULL,
    descricao VARCHAR(10),
    realizada_em TIMESTAMP WITHOUT TIME ZONE NOT NULL,
    cliente_id INT NOT NULL,
    CONSTRAINT fk_cliente
        FOREIGN KEY(cliente_id) 
        REFERENCES public.clientes(id)
);

INSERT INTO public.clientes (id, limite, saldo)
VALUES
(1, 100000, 0),
(2, 80000, 0),
(3, 1000000, 0),
(4, 10000000, 0),
(5, 500000, 0)
ON CONFLICT (id) DO NOTHING;
