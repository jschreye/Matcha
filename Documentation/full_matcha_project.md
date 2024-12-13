
# Projet Matcha Web

**Résumé** : Parce que l'amour aussi peut être industrialisé.

**Version** : 4.2

## Table des matières
I Avant-propos  
II Introduction  
III Instructions générales  
IV Partie obligatoire  
- IV.1 Inscription et Connexion  
- IV.2 Profil utilisateur  
- IV.3 Navigation  
- IV.4 Recherche  
- IV.5 Profil des autres utilisateurs  
- IV.6 Discussion instantanée  
- IV.7 Notifications  
V Partie bonus  
VI Soumission et évaluation par les pairs  
- VI.1 Évaluation par les pairs

---

### Chapitre I - Avant-propos

Ce deuxième millénaire a changé à jamais les habitudes et les manières d'utiliser Internet. Le choix est guidé par les technologies, et la place laissée à la chance est de plus en plus petite. Les relations humaines, semence de toute société moderne, sont de plus en plus créées artificiellement par des algorithmes de sites de rencontres et des réseaux sociaux, entre des personnes correspondant à des critères très précis.

Oui, le romantisme est mort, et Victor Hugo doit probablement se retourner dans sa tombe.

---

### Chapitre II - Introduction

Ce projet vise à créer un site de rencontres.

Vous devrez créer une application permettant à deux potentiels amoureux de se rencontrer, de l'inscription jusqu'à la rencontre finale.

Les utilisateurs pourront s'inscrire, se connecter, compléter leur profil, rechercher et voir les profils d'autres utilisateurs, et montrer leur intérêt en envoyant un "like". Trouvez un terme plus explicite que "like" pour cette action.

---

### Chapitre III - Instructions générales

- Votre application ne doit comporter aucune erreur, avertissement ou notification, que ce soit côté serveur ou côté client.
- Pour ce projet, vous pouvez utiliser n'importe quel langage de programmation de votre choix.
- Vous pouvez utiliser des micro-frameworks et toutes les bibliothèques nécessaires à ce projet.
- Vous pouvez utiliser des bibliothèques d'interface utilisateur telles que React, Angular, Vue, Bootstrap, Semantic, ou toute combinaison de celles-ci.
- Aucune faille de sécurité n'est autorisée. Vous devez au minimum respecter les exigences de la partie obligatoire, mais nous vous encourageons à aller plus loin. Tout en dépend.
- Un "micro-framework" est défini ici comme un framework contenant un routeur et éventuellement un moteur de templates, mais ne contenant pas d'ORM, de validateurs ou de gestionnaire de comptes utilisateurs.
- Vous devez utiliser une base de données relationnelle ou orientée graphe, telle que MySQL, MariaDB, PostgreSQL, Cassandra, InfluxDB, Neo4j, etc. Vous devrez écrire vos requêtes manuellement comme le ferait un développeur confirmé.
- Vous pouvez choisir le serveur Web qui vous convient le mieux, comme Apache, Nginx, ou un serveur Web intégré.
- Votre application doit être compatible avec les dernières versions de Firefox et Chrome.
- Le site doit avoir un layout correct : un header, une section principale et un footer.
- Votre site doit être utilisable sur mobile et avoir un design acceptable sur les petites résolutions.
- Toutes les formulaires doivent avoir des validations appropriées et l'ensemble du site doit être sécurisé.

Les éléments suivants sont considérés comme non sécurisés :
- Stocker des mots de passe en texte brut dans la base de données.
- Permettre l'injection de code HTML ou JavaScript dans des variables non protégées.
- Permettre le téléchargement de contenu non désiré.
- Permettre l'altération des requêtes SQL.

---

### Chapitre IV - Partie obligatoire

Vous devez créer une application Web avec les fonctionnalités suivantes :

#### IV.1 Inscription et Connexion

L'application doit permettre à un utilisateur de s'inscrire en demandant au moins son adresse email, son nom d'utilisateur, son nom de famille, son prénom, et un mot de passe protégé. Après l'inscription, un email avec un lien unique doit être envoyé à l'utilisateur pour vérifier son compte.

L'utilisateur doit pouvoir se connecter avec son nom d'utilisateur et son mot de passe, ainsi que recevoir un email pour réinitialiser son mot de passe en cas d'oubli. L'utilisateur doit également pouvoir se déconnecter d'un seul clic depuis n'importe quelle page du site.

#### IV.2 Profil utilisateur

Une fois connecté, l'utilisateur doit remplir son profil avec les informations suivantes :
- Le genre.
- Les préférences sexuelles.
- Une biographie.
- Une liste d'intérêts sous forme de tags (par exemple #vegan, #geek, #piercing), réutilisables.
- Jusqu'à 5 photos, dont une à utiliser comme photo de profil.

À tout moment, l'utilisateur doit pouvoir modifier ces informations ainsi que son nom, prénom et adresse email.  
L'utilisateur doit pouvoir voir qui a consulté son profil et qui l'a "liké".  
L'utilisateur doit avoir un "score de popularité" public, dont les critères sont à définir par vous.

L'utilisateur doit être localisé via GPS, jusqu'à son quartier. Si l'utilisateur ne souhaite pas être localisé, vous devez trouver un moyen de le localiser sans qu'il le sache. L'utilisateur doit pouvoir modifier sa position GPS dans son profil.

#### IV.3 Navigation

L'utilisateur doit pouvoir obtenir facilement une liste de suggestions correspondant à son profil.

Vous proposerez uniquement des profils "intéressants". Par exemple, uniquement des hommes pour une femme hétérosexuelle. Vous devez également gérer la bisexualité. Si l'orientation de l'utilisateur n'est pas spécifiée, il sera considéré comme bisexuel.

Vous devez matcher les utilisateurs intelligemment selon :
- La même zone géographique que l'utilisateur.
- Un maximum de tags en commun.
- Un "score de popularité" élevé.

La liste doit être triée par âge, localisation, "score de popularité" et tags communs. Elle doit également pouvoir être filtrée par ces critères.

#### IV.4 Recherche

L'utilisateur doit pouvoir effectuer une recherche avancée en sélectionnant un ou plusieurs critères :
- Une tranche d'âge.
- Un "score de popularité" minimal.
- Une localisation.
- Un ou plusieurs tags d'intérêt.

La liste des résultats doit être triée et filtrée par âge, localisation, "score de popularité" et tags.

#### IV.5 Profil des autres utilisateurs

Un utilisateur doit pouvoir consulter les profils des autres utilisateurs, contenant toutes les informations disponibles sauf l'adresse email et le mot de passe.

Lorsqu'un utilisateur consulte un profil, cela doit être ajouté à son historique de visites.  
L'utilisateur doit pouvoir "liker" la photo de profil d'un autre utilisateur. Si deux utilisateurs se "likent", ils sont considérés comme "connectés" et peuvent discuter. Un utilisateur sans photo de profil ne peut pas "liker".  
L'utilisateur doit également pouvoir retirer son "like", ce qui met fin à la connexion et aux notifications entre les deux utilisateurs.  
L'utilisateur doit pouvoir voir si un autre utilisateur est en ligne ou non, et voir la date et l'heure de sa dernière connexion.  
L'utilisateur doit pouvoir signaler un compte comme "faux" et bloquer un autre utilisateur.

#### IV.6 Discussion instantanée

Lorsque deux utilisateurs sont connectés, ils doivent pouvoir discuter en temps réel avec un délai maximal de 10 secondes. L'implémentation du chat est libre.

#### IV.7 Notifications

Un utilisateur doit être notifié en temps réel dans les cas suivants :
- Lorsqu'il reçoit un "like".
- Lorsqu'un utilisateur consulte son profil.
- Lorsqu'il reçoit un message.
- Lorsque l'utilisateur "liké" lui renvoie un "like".
- Lorsqu'un utilisateur connecté retire son "like".

---

### Chapitre V - Partie bonus

Vous pouvez implémenter des fonctionnalités bonus pour obtenir des points supplémentaires, comme :
- Ajouter des stratégies Omniauth pour l'authentification.
- Permettre l'importation de photos depuis les réseaux sociaux (Snapchat, Facebook, Google+, etc.).
- Développer une carte interactive des utilisateurs nécessitant une localisation GPS plus précise.
- Intégrer un chat audio ou vidéo pour les utilisateurs connectés.
- Mettre en place une fonctionnalité pour organiser des rencontres en personne pour les utilisateurs matchés.

La partie bonus ne sera évaluée que si la partie obligatoire est parfaitement réalisée. Par "parfaitement", cela signifie que la partie obligatoire a été complètement implémentée et fonctionne sans aucune défaillance.

---

### Chapitre VI - Soumission et évaluation par les pairs

Soumettez votre projet dans votre dépôt Git habituel. Seul le travail dans le dépôt sera évalué lors de la soutenance. Vérifiez bien les noms de vos fichiers et dossiers pour vous assurer qu'ils sont corrects.

#### VI.1 Évaluation par les pairs

Votre code ne doit générer aucune erreur, avertissement ou notification, que ce soit côté serveur ou client.  
Toute faille de sécurité entraînera une note de 0.

