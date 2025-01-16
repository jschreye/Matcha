CREATE TABLE IF NOT EXISTS users (
    id INT AUTO_INCREMENT PRIMARY KEY,
    lastname VARCHAR(50) NOT NULL,
    firstname VARCHAR(50) NOT NULL,
    username VARCHAR(50) NOT NULL UNIQUE,
    email VARCHAR(100) NOT NULL UNIQUE,
    password_hash VARCHAR(255),
    isactive boolean,
    activationtoken VARCHAR(255),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

INSERT INTO users (firstname, lastname, username, email) VALUES
('a', 'a', 'Alice', 'alice@example.com'),
('b', 'b', 'Bob', 'bob@example.com'),
('c' ,'c' ,'Charlie', 'charlie@example.com'),
('d', 'd', 'Diana', 'diana@example.com'),
('e', 'e', 'Eve', 'eve@example.com');