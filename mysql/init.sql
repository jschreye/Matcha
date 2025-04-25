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

CREATE TABLE IF NOT EXISTS users (
    id INT AUTO_INCREMENT PRIMARY KEY,
    email VARCHAR(100) NOT NULL UNIQUE,
    age INT,
    username VARCHAR(50) NOT NULL UNIQUE,
    lastname VARCHAR(50) NOT NULL,
    firstname VARCHAR(50) NOT NULL, 
    password_hash VARCHAR(255) NOT NULL,
    genre_id INT,
    tag_id INT,                
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
    FOREIGN KEY (sexual_preferences_id) REFERENCES prefsex(id),
    FOREIGN KEY (tag_id) REFERENCES tags(id)
) ENGINE=InnoDB;

-- Table Photos
CREATE TABLE IF NOT EXISTS photos (
    id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT NOT NULL,
    image_data LONGBLOB NOT NULL,
    est_profil BOOLEAN DEFAULT FALSE,
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
) ENGINE=InnoDB;

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
('Unlike'),
('Visite'),
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
CREATE TABLE IF NOT EXISTS `sessions` (
    id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT NOT NULL,
    session_token VARCHAR(255) NOT NULL UNIQUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    expires_at TIMESTAMP,
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
) ENGINE=InnoDB;

-- Script pour seeder 20 utilisateurs fictifs pour tester les fonctionnalités de recherche
-- Tous les mots de passe sont 'Password123!'
-- Hash généré correspondant à 'Password123!'
SET @password_hash = '$2a$11$K3.BCTZyTSAQxdTH8bh.DOD.bsvW/Hk8.pnh0TJxMOo4vQxfDXnv.';

-- Insérer des utilisateurs fictifs
INSERT INTO users (username, firstname, lastname, email, password_hash, age, biography, genre_id, sexual_preferences_id, tag_id, gps_location, popularity_score, profile_complete, isactive, localisation_isactive, notifisactive, created_at)
VALUES 
-- Femmes intéressées par les hommes
('sophie92', 'Sophie', 'Dupont', 'sophie@example.com', @password_hash, 28, 'Amoureuse de la vie et des voyages. Je cherche une personne sincère pour partager de bons moments.', 2, 1, 3, POINT(2.3522, 48.8566), 75, 1, 1, 1, 1, NOW()),
('laura123', 'Laura', 'Martin', 'laura@example.com', @password_hash, 24, 'Passionnée de photographie et de randonnée. J''aime explorer de nouveaux endroits.', 2, 1, 5, POINT(2.3512, 48.8576), 82, 1, 1, 1, 1, NOW()),
('chloe_paris', 'Chloé', 'Bernard', 'chloe@example.com', @password_hash, 31, 'Médecin passionnée par mon métier. Je cherche quelqu''un qui partage ma passion pour la vie.', 2, 1, 7, POINT(2.3532, 48.8556), 68, 1, 1, 1, 1, NOW()),
('julia87', 'Julia', 'Robert', 'julia@example.com', @password_hash, 26, 'Je suis musicienne et j''adore les concerts. Cherche quelqu''un pour m''accompagner dans mes aventures.', 2, 1, 1, POINT(2.3542, 48.8546), 90, 1, 1, 1, 1, NOW()),
('emma_lyon', 'Emma', 'Blanc', 'emma@example.com', @password_hash, 30, 'Avocate, je cherche un équilibre entre vie professionnelle et personnelle. Je suis sportive et j''adore cuisiner.', 2, 1, 9, POINT(4.8357, 45.7640), 65, 1, 1, 1, 1, NOW()),

-- Hommes intéressés par les femmes
('thomas75', 'Thomas', 'Dubois', 'thomas@example.com', @password_hash, 32, 'Ingénieur en informatique, je suis un geek passionné par les nouvelles technologies et le cinéma.', 1, 2, 2, POINT(2.3502, 48.8586), 70, 1, 1, 1, 1, NOW()),
('nicolas_m', 'Nicolas', 'Moreau', 'nicolas@example.com', @password_hash, 29, 'Chef cuisinier, j''aime créer et partager. Cherche une personne authentique pour une relation sincère.', 1, 2, 8, POINT(2.3492, 48.8596), 85, 1, 1, 1, 1, NOW()),
('antoine33', 'Antoine', 'Leroy', 'antoine@example.com', @password_hash, 35, 'Photographe professionnel, passionné de voyage et d''aventure. Je vis chaque jour comme une nouvelle découverte.', 1, 2, 4, POINT(2.3482, 48.8606), 60, 1, 1, 1, 1, NOW()),
('hugo_paris', 'Hugo', 'Girard', 'hugo@example.com', @password_hash, 27, 'Architecte, j''aime les belles choses et l''art en général. Cherche une relation basée sur la complicité.', 1, 2, 6, POINT(2.3472, 48.8616), 78, 1, 1, 1, 1, NOW()),
('lucas_lyon', 'Lucas', 'Petit', 'lucas@example.com', @password_hash, 33, 'Entrepreneur dans l''âme, j''aime les défis et les personnes qui me poussent vers l''avant.', 1, 2, 9, POINT(4.8347, 45.7650), 72, 1, 1, 1, 1, NOW()),

-- Profils mixtes avec diverses préférences
('alex_nonbinary', 'Alex', 'Rousseau', 'alex@example.com', @password_hash, 25, 'Non-binaire, artiste, je suis ouvert(e) à rencontrer des personnes intéressantes, quelle que soit leur identité.', 3, 3, 3, POINT(2.3462, 48.8626), 80, 1, 1, 1, 1, NOW()),
('camille_bi', 'Camille', 'Simon', 'camille@example.com', @password_hash, 29, 'Bisexuel(le), enseignant(e), j''adore les voyages et les découvertes culturelles.', 3, 3, 5, POINT(2.3452, 48.8636), 75, 1, 1, 1, 1, NOW()),
('maxime_gay', 'Maxime', 'Fournier', 'maxime@example.com', @password_hash, 31, 'Homme gay, ingénieur, passionné de théâtre et de littérature. Je cherche une relation sérieuse.', 1, 2, 7, POINT(2.3442, 48.8646), 68, 1, 1, 1, 1, NOW()),
('sarah_lesbian', 'Sarah', 'Lambert', 'sarah@example.com', @password_hash, 28, 'Femme lesbienne, médecin, j''aime le sport, la nature et les animaux.', 2, 2, 9, POINT(2.3432, 48.8656), 70, 1, 1, 1, 1, NOW()),
('morgan_fluid', 'Morgan', 'Mercier', 'morgan@example.com', @password_hash, 26, 'Genre fluide, musicien(ne), passionné(e) de cuisine et de voyage.', 3, 3, 1, POINT(2.3422, 48.8666), 82, 1, 1, 1, 1, NOW()),

-- Profils avec intérêts spécifiques
('kevin_gaming', 'Kevin', 'Roux', 'kevin@example.com', @password_hash, 24, 'Passionné par les jeux vidéo et l''univers geek. Je cherche quelqu''un qui partage ces passions.', 1, 2, 1, POINT(5.3697, 43.2965), 65, 1, 1, 1, 1, NOW()),
('marie_art', 'Marie', 'Vincent', 'marie@example.com', @password_hash, 27, 'Artiste peintre, je vis pour l''art et la création. J''aime les esprits créatifs et originaux.', 2, 1, 4, POINT(5.3687, 43.2975), 78, 1, 1, 1, 1, NOW()),
('paul_sport', 'Paul', 'Muller', 'paul@example.com', @password_hash, 30, 'Sportif professionnel, je suis dynamique et plein d''énergie. J''aime les personnes qui me suivent dans mes aventures.', 1, 2, 6, POINT(5.3677, 43.2985), 85, 1, 1, 1, 1, NOW()),
('julie_nature', 'Julie', 'Garnier', 'julie@example.com', @password_hash, 29, 'Biologiste marine, j''ai une passion pour la nature et l''écologie. Je cherche quelqu''un qui partage ces valeurs.', 2, 1, 8, POINT(5.3667, 43.2995), 70, 1, 1, 1, 1, NOW()),
('david_music', 'David', 'Faure', 'david@example.com', @password_hash, 33, 'Compositeur, la musique est ma vie. Je cherche une personne qui vibre au même rythme que moi.', 1, 2, 7, POINT(5.3657, 43.3005), 76, 1, 1, 1, 1, NOW());

-- Créer quelques relations (likes) entre utilisateurs pour générer des scores de popularité
INSERT INTO likes (user_id, liked_user_id, timestamp)
VALUES
-- User 6 (Thomas) like Sophie, Laura
(6, 1, NOW()),
(6, 2, NOW()),

-- User 7 (Nicolas) like Sophie
(7, 1, NOW()),

-- User 8 (Antoine) like Sophie, Chloé
(8, 1, NOW()),
(8, 3, NOW()),

-- User 9 (Hugo) like Laura, Julia
(9, 2, NOW()),
(9, 4, NOW()),

-- Quelques autres likes pour créer un réseau
(1, 6, NOW()),  -- Sophie like Thomas (match)
(3, 8, NOW()),  -- Chloé like Antoine (match)
(4, 7, NOW()),  -- Julia like Nicolas
(5, 9, NOW()),  -- Emma like Hugo
(11, 12, NOW()),-- Alex like Camille
(13, 14, NOW()),-- Maxime like Sarah
(16, 17, NOW()),-- Kevin like Marie
(19, 18, NOW());-- Paul like Julie

-- Note: Pour ajouter des photos aux profils utilisateurs, vous devrez exécuter l'application
-- et uploader des photos manuellement ou créer un script qui convertit des images en BLOB.
-- 
-- Exemple (non fonctionnel ici) de ce que serait l'ajout d'une photo si nous avions le BLOB de l'image:
-- INSERT INTO photos (user_id, image_data, est_profil) VALUES (1, [BINARY_DATA_HERE], 1);