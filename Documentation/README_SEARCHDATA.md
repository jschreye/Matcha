# Données de test pour la recherche de profils

## Implémentation du test

Les données de test pour la fonctionnalité de recherche de profils ont été intégrées directement dans le fichier `mysql/init.sql`. Cela signifie que les utilisateurs de test seront automatiquement créés lors de l'initialisation de la base de données.

## Profils inclus

Le jeu de données inclut 20 utilisateurs fictifs avec des profils complets :

### Répartition des profils
- 5 femmes intéressées par les hommes
- 5 hommes intéressés par les femmes 
- 5 profils mixtes (non-binaire, bisexuel·le, etc.)
- 5 profils avec des intérêts spécifiques (jeux vidéo, art, sport, etc.)

### Diversité des données
- Âges variés (24-35 ans)
- Scores de popularité différents (60-90)
- Localisations géographiques diverses (Paris, Lyon, Marseille)
- Tags d'intérêt variés
- Différentes combinaisons de genre et préférences sexuelles

### Réseau de relations
Le script génère également un réseau de "likes" entre les utilisateurs, permettant de tester les matches et d'autres fonctionnalités sociales.

## Photos de profil

Le schéma de base de données utilise des BLOB pour stocker les images, ce qui signifie que les photos de profil devront être ajoutées manuellement via l'application. Les données de test ne contiennent pas les photos.

## Informations d'authentification

Tous les utilisateurs fictifs ont le même mot de passe :
- Mot de passe : `Password123!`

## Fonctionnalités testables

Ces données permettent de tester les fonctionnalités de recherche suivantes :
1. Recherche de profils similaires (basée sur les centres d'intérêt, l'âge, la localisation)
2. Recherche de profils aléatoires
3. Recherche personnalisée avec filtres (âge, popularité, distance, tags)

## Nettoyage des données

Pour supprimer ces utilisateurs de test (si nécessaire), vous pouvez exécuter :

```sql
DELETE FROM likes WHERE user_id IN (SELECT id FROM users WHERE email LIKE '%@example.com');
DELETE FROM users WHERE email LIKE '%@example.com';
``` 