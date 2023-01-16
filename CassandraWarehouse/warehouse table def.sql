CREATE TABLE IF NOT EXISTS stocks (
    ware UUID,
    quantity counter,
    receiving UUID,
    PRIMARY KEY(ware, receiving)
);

CREATE TABLE IF NOT EXISTS wares (     
    id UUID,
    name text,
    PRIMARY KEY(id, name)
);

CREATE TABLE IF NOT EXISTS receivings (
    id UUID,
    number text,
    date date,
    client text,
    positions text,
    PRIMARY KEY(date, id)
);

CREATE TABLE IF NOT EXISTS releases (
    id UUID,
    number text,
    date date,
    client text,
    positions text,
    PRIMARY KEY(date, id)
);