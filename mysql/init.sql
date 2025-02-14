CREATE TABLE IF NOT EXISTS genre (
    id INT PRIMARY KEY AUTO_INCREMENT,
    libelle VARCHAR(20) NOT NULL UNIQUE
) ENGINE=InnoDB;

-- 2. Insérer les données dans la table genre
INSERT INTO genre (libelle) VALUES
('Homme'),
('Femme'),
('Autre');


CREATE TABLE IF NOT EXISTS prefsex (
    id INT PRIMARY KEY AUTO_INCREMENT,
    libelle VARCHAR(20) NOT NULL UNIQUE
) ENGINE=InnoDB;

INSERT INTO prefsex (libelle) VALUES
('Hetero'),
('Gay'),
('Bisexual');

CREATE TABLE IF NOT EXISTS users (
    id INT AUTO_INCREMENT PRIMARY KEY,
    email VARCHAR(100) NOT NULL UNIQUE,
    username VARCHAR(50) NOT NULL UNIQUE,
    lastname VARCHAR(50) NOT NULL,
    firstname VARCHAR(50) NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    genre_id INT,                -- Genre de l'utilisateur
    sexual_preferences_id INT,   -- Préférences sexuelles
    biography TEXT,                    -- Biographie
    gps_location POINT,                -- Localisation GPS (optionnel, peut utiliser lat/long séparément)
    popularity_score INT DEFAULT 0,    -- Score de popularité
    isactive BOOLEAN DEFAULT FALSE,
    activationtoken VARCHAR(255),
    passwordresettoken VARCHAR(255),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    profile_complete BOOLEAN DEFAULT FALSE,
    localisation_isactive BOOLEAN DEFAULT FALSE,
    FOREIGN KEY (genre_id) REFERENCES genre(id),
    FOREIGN KEY (sexual_preferences_id) REFERENCES prefsex(id)
) ENGINE=InnoDB;

-- Table Photos
CREATE TABLE IF NOT EXISTS photos (
    id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT NOT NULL,
    image_data LONGBLOB NOT NULL,   -- Stockage binaire de la photo
    est_profil BOOLEAN DEFAULT FALSE,
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
) ENGINE=InnoDB;

-- Table Tags
CREATE TABLE IF NOT EXISTS tags (
    id INT AUTO_INCREMENT PRIMARY KEY,
    libelle VARCHAR(50) NOT NULL UNIQUE
) ENGINE=InnoDB;

INSERT INTO tags (libelle) VALUES
('Jeux video'),
('Grimpe'),
('Shopping'),
('Course a pied'),
('Plaide et chocolat chaud'),
('Rando'),
('Volley'),
('Foot'),
('Ski');

-- Table UserTags (relation plusieurs-à-plusieurs entre Users et Tags)
CREATE TABLE IF NOT EXISTS userTags (
    user_id INT NOT NULL,
    tag_id INT NOT NULL,
    PRIMARY KEY (user_id, tag_id),
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
    FOREIGN KEY (tag_id) REFERENCES tags(id) ON DELETE CASCADE
) ENGINE=InnoDB;

-- Table Likes
CREATE TABLE IF NOT EXISTS likes (
    user_id INT NOT NULL,
    liked_user_id INT NOT NULL,
    timestamp TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (user_id, liked_user_id),
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
    FOREIGN KEY (liked_user_id) REFERENCES users(id) ON DELETE CASCADE
) ENGINE=InnoDB;

-- Table Visits
CREATE TABLE IF NOT EXISTS visits (
    user_id INT NOT NULL,
    visited_user_id INT NOT NULL,
    timestamp TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (user_id, visited_user_id, timestamp),
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
    FOREIGN KEY (visited_user_id) REFERENCES users(id) ON DELETE CASCADE
) ENGINE=InnoDB;

-- Table Messages
CREATE TABLE IF NOT EXISTS messages (
    id INT AUTO_INCREMENT PRIMARY KEY,
    sender_id INT NOT NULL,
    receiver_id INT NOT NULL,
    contenu TEXT NOT NULL,
    timestamp TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (sender_id) REFERENCES users(id) ON DELETE CASCADE,
    FOREIGN KEY (receiver_id) REFERENCES users(id) ON DELETE CASCADE
) ENGINE=InnoDB;

-- Table Notifications
CREATE TABLE IF NOT EXISTS notifications (
    id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT NOT NULL,
    type VARCHAR(50),
    contenu TEXT,
    lu BOOLEAN DEFAULT FALSE,
    timestamp TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
) ENGINE=InnoDB;

-- Table BlocksReports (pour blocages et rapports)
CREATE TABLE IF NOT EXISTS blocksReports (
    user_id INT NOT NULL,
    blocked_user_id INT NOT NULL,
    report_reason TEXT,
    timestamp TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (user_id, blocked_user_id),
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
    FOREIGN KEY (blocked_user_id) REFERENCES users(id) ON DELETE CASCADE
) ENGINE=InnoDB;

-- Table Sessions (pour gérer les connexions des utilisateurs)
CREATE TABLE IF NOT EXISTS sessions (
    id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT NOT NULL,
    session_token VARCHAR(255) NOT NULL UNIQUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    expires_at TIMESTAMP,
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
) ENGINE=InnoDB;

-- Insertion d'exemples d'utilisateurs
INSERT INTO users (firstname, lastname, username, email, password_hash)
VALUES
('a', 'a', 'Alice', 'alice@example.com', 'hashed_password'),
('b', 'b', 'Bob', 'bob@example.com', 'hashed_password'),
('c', 'c', 'Charlie', 'charlie@example.com', 'hashed_password'),
('d', 'd', 'Diana', 'diana@example.com', 'hashed_password'),
('e', 'e', 'Eve', 'eve@example.com', 'hashed_password');

-- Insertion d'exemples de photos
INSERT INTO photos (user_id, image_data, est_profil)
VALUES
(1, 0xFFD8FFE0, TRUE),
(2, 0xFFD8FFE1, TRUE),
(3, 0xFFD8FFE2, FALSE),
(4, 0xFFD8FFE3, FALSE),
(5, 0xFFD8FFE4, TRUE);

-- Insertion d'exemples de relations Users-Tags
INSERT INTO userTags (user_id, tag_id)
VALUES
(1, 1),
(1, 2),
(2, 2),
(2, 3),
(3, 3),
(3, 4),
(4, 4),
(4, 5),
(5, 1),
(5, 5);

-- Insertion d'exemples de likes
INSERT INTO likes (user_id, liked_user_id)
VALUES
(1, 2),
(2, 3),
(3, 1),
(4, 5),
(5, 4);

-- Insertion d'exemples de visites
INSERT INTO visits (user_id, visited_user_id)
VALUES
(1, 3),
(2, 1),
(3, 2),
(4, 1),
(5, 2);

-- Insertion d'exemples de messages
INSERT INTO messages (sender_id, receiver_id, contenu)
VALUES
(1, 2, 'Hi Bob!'),
(2, 1, 'Hello Alice!'),
(3, 4, 'Hey Diana!'),
(4, 5, 'Hello Eve!'),
(5, 1, 'Hi Alice!');

-- Insertion d'exemples de notifications
INSERT INTO notifications (user_id, type, contenu)
VALUES
(1, 'message', 'You have a new message from Bob.'),
(2, 'like', 'Alice liked your profile.'),
(3, 'visit', 'Diana visited your profile.'),
(4, 'report', 'Your profile has been reported.'),
(5, 'block', 'Bob has blocked you.');

-- Insertion d'exemples de blocages/rapports
INSERT INTO blocksReports (user_id, blocked_user_id, report_reason)
VALUES
(1, 4, 'Inappropriate behavior'),
(2, 5, 'Spam'),
(3, 2, 'Harassment');

-- Insertion d'exemples de sessions
INSERT INTO sessions (user_id, session_token, expires_at)
VALUES
(1, 'token_abc123', DATE_ADD(NOW(), INTERVAL 1 DAY)),
(2, 'token_def456', DATE_ADD(NOW(), INTERVAL 1 DAY)),
(3, 'token_ghi789', DATE_ADD(NOW(), INTERVAL 1 DAY)),
(4, 'token_jkl012', DATE_ADD(NOW(), INTERVAL 1 DAY)),
(5, 'token_mno345', DATE_ADD(NOW(), INTERVAL 1 DAY));
