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
    age INT,
    username VARCHAR(50) NOT NULL UNIQUE,
    lastname VARCHAR(50) NOT NULL,
    firstname VARCHAR(50) NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    genre_id INT,                
    sexual_preferences_id INT, 
    biography TEXT,
    gps_location POINT,
    popularity_score INT DEFAULT 0,
    isactive BOOLEAN DEFAULT FALSE,
    notifisactive BOOLEAN DEFAULT FALSE,
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
    image_data LONGBLOB NOT NULL,
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

-- Table Matches

CREATE TABLE IF NOT EXISTS matches (
    id INT AUTO_INCREMENT PRIMARY KEY,
    user1_id INT NOT NULL,
    user2_id INT NOT NULL,
    matched_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    is_active BOOLEAN DEFAULT TRUE,
    FOREIGN KEY (user1_id) REFERENCES users(id) ON DELETE CASCADE,
    FOREIGN KEY (user2_id) REFERENCES users(id) ON DELETE CASCADE,
    UNIQUE KEY unique_match (user1_id, user2_id)
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

CREATE TABLE IF NOT EXISTS notification_types (
    id INT AUTO_INCREMENT PRIMARY KEY,
    libelle VARCHAR(50) NOT NULL UNIQUE
) ENGINE=InnoDB;

INSERT INTO notification_types (libelle) VALUES
('Message'),
('Like'),
('Match');

-- Table Notifications
CREATE TABLE IF NOT EXISTS notifications (
    id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT NOT NULL,
    sender_id INT NOT NULL,
    notification_type_id INT,
    lu BOOLEAN DEFAULT FALSE,
    timestamp TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
    FOREIGN KEY (notification_type_id) REFERENCES notification_types(id) ON DELETE SET NULL
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