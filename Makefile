# COLORS
GREEN		= \033[1;32m
RED 		= \033[1;31m
ORANGE		= \033[1;33m
CYAN		= \033[1;36m
RESET		= \033[0m

# Dossiers
SRCS_DIR	= ./
ENV_FILE	= ${SRCS_DIR}.env

# Fichiers Docker Compose
DOCKER_COMPOSE_PROD	= docker-compose.yml
DOCKER_COMPOSE_DEV	= docker-compose.dev.yml

# Noms des projets Docker Compose
PROJECT_NAME_PROD = matcha-prod
PROJECT_NAME_DEV  = matcha-dev

# Commandes Docker Compose
DOCKER_PROD = docker compose -f ${DOCKER_COMPOSE_PROD} --env-file ${ENV_FILE} -p ${PROJECT_NAME_PROD}
DOCKER_DEV  = docker compose -f ${DOCKER_COMPOSE_PROD} -f ${DOCKER_COMPOSE_DEV} --env-file ${ENV_FILE} -p ${PROJECT_NAME_DEV}

# Cible par défaut
all: up-prod

# Commandes pour la production
up-prod:
	@echo "${GREEN}Démarrage des conteneurs de production...${RESET}"
	@${DOCKER_PROD} up -d --remove-orphans

down-prod:
	@echo "${RED}Arrêt des conteneurs de production...${RESET}"
	@${DOCKER_PROD} down

stop-prod:
	@echo "${RED}Arrêt des conteneurs de production...${RESET}"
	@${DOCKER_PROD} stop

rebuild-prod:
	@echo "${GREEN}Reconstruction des conteneurs de production...${RESET}"
	@${DOCKER_PROD} up -d --remove-orphans --build

delete-prod:
	@echo "${RED}Suppression des conteneurs de production...${RESET}"
	@${DOCKER_PROD} down -v --remove-orphans

rebuild-no-cache-prod:
	@echo "${GREEN}Reconstruction des conteneurs de production sans cache...${RESET}"
	@${DOCKER_PROD} build --no-cache
	@${DOCKER_PROD} up -d --remove-orphans --build

# Commandes pour le développement
up-dev:
	@echo "${GREEN}Démarrage des conteneurs de développement...${RESET}"
	@${DOCKER_DEV} up --build

down-dev:
	@echo "${RED}Arrêt des conteneurs de développement...${RESET}"
	@${DOCKER_DEV} down

stop-dev:
	@echo "${RED}Arrêt des conteneurs de développement...${RESET}"
	@${DOCKER_DEV} stop

rebuild-dev:
	@echo "${GREEN}Reconstruction des conteneurs de développement...${RESET}"
	@${DOCKER_DEV} up -d --remove-orphans --build

delete-dev:
	@echo "${RED}Suppression des conteneurs de développement...${RESET}"
	@${DOCKER_DEV} down -v --remove-orphans

rebuild-no-cache-dev:
	@echo "${GREEN}Reconstruction des conteneurs de développement sans cache...${RESET}"
	@${DOCKER_DEV} build --no-cache
	@${DOCKER_DEV} up -d --remove-orphans --build


# Cibles Phony
.PHONY: all up-prod down-prod stop-prod rebuild-prod delete-prod rebuild-no-cache-prod \
        up-dev down-dev stop-dev rebuild-dev delete-dev rebuild-no-cache-dev