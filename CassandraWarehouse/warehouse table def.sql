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
    date timestamp,
    client text,
    positions text,
    PRIMARY KEY(date, id)
);

CREATE TABLE IF NOT EXISTS releases (
    number text,
    date timestamp,
    client text,
    positions text,
    PRIMARY KEY(date, id)
);