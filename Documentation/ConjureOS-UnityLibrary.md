# Guide d'utilisation de la Unity Library
Ce guide sert à démontrer les différentes étapes qui doivent être effectuées dans Unity afin de mettre en ligne un jeu sur les serveurs web de Conjure. Les différentes fonctionnalités offertes par l’éditeur de métadonnées, ainsi que quelques outils de débogages simples, seront aussi présentées.

## 1. Éditeur de métadonnées
Pour qu'un jeu puisse être déployé sur l'arcade, certaines informations doivent être remplies. Il est possible de modifier ces informations par la fenêtre **Game Metadata Editor**, accessible via le menu *Conjure Arcade > Game Metadata Editor*.

### 1.1 Onglet *Metadata Editor*
Cette section permet d'éditer les différents champs de métadonnées de votre jeu.

![Metadata Editor Tab 1 Preview](Docs/Images/MetadataEditor-Tab1-Preview.png)

#### Champs de métadonnées
Voici une description des différents champs, ainsi que leur restriction:
- **Game Title:** le nom de votre jeu. Ne peut être un champ vide, et doit faire moins de 100 caractères.
- **Version:** la version actuelle de votre jeu. Par défaut, la version débute à "1.0.0". Le bouton sélectionné indique l'importance de la mise à jour. Un aperçu de la prochaine version est aussi visible. Après la mise en ligne de votre jeu, la version actuelle évoluera.
- **Description:** courte description du jeu. Doit faire moins de 250 caractères.
- **Min/Max Number of Players:** détermine le nombre de joueur minimum/maximum requis pour jouer au jeu.
- **Use Leaderboard:** indique si le jeu utilise le système de classement de l'arcade.
- **Thumbnail:** image principale du jeu qui sera visible dans l'écran de sélection des jeux sur l'arcade. L'image doit être au format 16:9, et doit peser moins de 1 Mo.
- **Gameplay Image:** image de démonstration du jeu. Doit être au format 1:1, et doit peser moins de 1 Mo.
- **Developers:** liste des personnes ayant participées aux développement du jeu. Vous pouvez ajouter un nom, le supprimer ou réordonner la liste selon le besoin.
- **Genres:** liste des genres associés au jeu. Un maximum de trois genres peut être sélectionné à la fois.
- **Public Repository Link:** *CHAMP OPTIONNEL*. Afin que l'équipe administrateur puisse mettre à jour le jeu si de nouvelles versions du Unity Library venaient à sortir, vous pouvez mettre un lien vers le repo publique de votre jeu.

#### Validation des données
Une fois les différents champs remplis, vous pouvez effectuer une validation des métadonnées en cliquant sur le bouton *Execute Validation*. Ce bouton sert à déterminer si les valeurs entrées sont conformes aux restrictions établis par le système d'arcade. Dans le cas où une ou plusieurs erreurs sont détectées, le ou les champs en question sont indiqués par un message.

![Metadata Editor Error Example](Docs/Images/MetadateEditor-ErrorExample.png)

À l'inverse, si aucune erreur n'est détectée, un message sera affiché au bas de la fenêtre indiquant que le jeu est prêt à être publié.

![Metadata Editor Validation Example](Docs/Images/MetadataEditor-ValidationExample.png)

**IMPORTANT:** la validation est normalement effectuée par le serveur web. Il est donc impératif d'être connecté au serveur (voir section **Connexion au serveur Conjure**) avant d'effectuer une validation. Autrement, la librairie Unity ne vérifiera que quelques cas de base et la validation sera donc incomplète.

### 1.2 Onglet *Online Metadata*
Cette section permet de récupérer et visualiser les métadonnées de votre jeu sur le serveur si celui-ci a déjà été mis en ligne. De ce fait, cette fonction n'est disponible que si vous êtes connecté au serveur Conjure. Les informations obtenues sont affichées dans un format brute.

![Metadata Editor Tab 2 Preview](Docs/Images/MetadataEditor-Tab2-Preview.png)

## 2. Nouvelles options dans les *Project Settings*
La Unity Library ajoute de nouvelles options dans les paramètres de projet (*Edit > Project Settings*).

![Project Settings Conjure](Docs/Images/ProjectSettings-Conjure.png)

### 2.1 Paramètres de leaderboard
Si vous utilisez le système de classement de la Unity Library, vous pouvez le paramétrer ici. La première option *Leaderboard Strategy* détermine la stratégie qui est utilisée par le système pour ce qui est de la lecture et de l'enregistrement des classements.

![Project Settings Leaderboard Strategy](Docs/Images/ProjectSettings-LeaderboardStrategy.png)

Voici une description de chacune des stratégies:
- **Local File:** les données de classement sont seulement lues et enregistrées localement.
- **Web Service:** les données de classement sont seulement lues et enregistrés sur le serveur de Conjure.
- **Fallback:** tente de sauvegarder et d'obtenir les données via la stratégie *Web Service*. Si le serveur n'est pas en ligne, la stratégie *Local File* est utilisée.

L'option suivante, soit *Leaderboard Sort Type*, détermine l'ordre d'affichage des scores.

![Project Settings Leaderboard SortType](Docs/Images/ProjectSettings-LeaderboardSortType.png)

### 2.2 Paramètres web
Pour le moment, un seul champ est disponible dans les paramètres web: le champ *Address*. Ce champ doit comporter l'adresse web du serveur Conjure. Si l'adresse est invalide, vous n'aurez pas accès aux différents services nécessitant une connexion au serveur. De même, si vous utilisez le système de classement et optez pour la stratégie *Web Service*, celle-ci ne sera pas fonctionnelle car elle requiert une connexion valide au serveur de Conjure.

## 3. Connexion au serveur Conjure
Afin de valider les métadonnées ou de mettre en ligne un jeu, vous devez être connecté au serveur Conjure. Les différents paramètres de connexion sont accessibles via le top menu *Conjure Arcade > Web Server Settings*.

![Web Server Settings Preview](Docs/Images/WebServerSettings-Preview.png)

Les informations de connexion doivent être ceux vous permettant de vous connecter à la plateforme web du serveur de l'arcade. Le nom d'utilisateur et le mot de passe ne sont pas sauvegardés. Vous pouvez ainsi partagez votre projet avec d'autres personnes sans problème.

Une fois connecté au serveur Conjure, quelques fonctionnalités supplémentaires sont désormais disponibles:
- Les métadonnées seront maintenant validés par le serveur, vous assurant ainsi de leur conformité;
- Possibilité de déployer le jeu sur l'arcade;
- Récupération des métadonnées du jeu s'il a déjà été publié.

## 4. Déploiement et mise en ligne
La mise en ligne du jeu s'effectue à partir de la fenêtre **Game Uploader**, qui est accessible via le top menu *Conjure Arcade > Upload Game*. Cette fenêtre est séparée en trois sections: *Metadata*, *Build and Upload* et *Utilities*.

### 4.1 Mise en ligne d'un jeu
Lorsque vous êtes prêt à déployer votre jeu sur le serveur Conjure, vous pouvez cliquer sur le bouton *Build & Upload*. Cela démarrera le processus de build du jeu et de sa mise en ligne.

![Game Uploader Build & Upload](Docs/Images/GameUploader-BuildAndUpload.png)

De la même manière que lorsque vous déployez un jeu, une fenêtre s'affichera vous permettant de sélectionner le dossier où le jeu sera construit. À noter qu'après le déploiement du jeu, celui-ci sera converti en un fichier portant l'extension *.conj*, et ne sera donc pas exécutable de manière traditionnelle.

![Game Uploader Build Window](Docs/Images/GameUploader-BuildWindow.png)

Après le déploiement et la mise en ligne du jeu, une fenêtre s'affichera vous indiquant que le processus a terminé avec succès. Une fenêtre d'erreur s'affichera aussi dans le cas inverse où un problème est survenu.

### 4.2 Autres fonctions
#### Section *Metadata*
Cette section comporte un seul bouton, soit le bouton *Validate Metadata*. Ce bouton a le même comportement que celui présent dans l'éditeur de métadonnées. Sa seule utilité est donc de pouvoir valider les métadonnées sans avoir à ouvrir la fenêtre d'édition.

![Game Uploader Metadata](Docs/Images/GameUploader-ValidateMetadata.png)

#### Section *Utilities*
Principalement utilisée par les développeurs de la Unity Library, les boutons *Generate Metadata.txt* et *Generate .conj file*, ces boutons permettent de générer respectivement un fichier *metadata.txt* ou un fichier *.conj* sans avoir à effectuer la processus de déploiement complet.

![Game Uploader Utilities](Docs/Images/GameUploader-Utilities.png)

## 5. Fichiers *.conj* et *metadata.txt*
 Un fichier *.conj*, qui est généré lors du déploiement d'un jeu vers le serveur (voir section 4), est un format de fichier qui est lu par la plateforme web et l'application de l'arcade. Malgré leur extension personnalisée, ce type de fichier est en réalité un amas de fichiers compressés au format ZIP. Il est donc possible d'analyser le contenu d'un tel fichier en l'ouvrant avec des outils, tel que 7-Zip.

 Un fichier *.conj* extrait en un dossier qui contient les éléments suivants à sa racine:
- Un fichier **metadata.txt** qui liste toutes les métadonnées du jeu. Les métadonnées entrées dans le *Metadata Editor* sont convertis en un fichier de ce type.
- Un sous-dossier **medias** qui contient les images de votre jeu (liés à partir du fichier metadata.txt).
- Un fichier ZIP **game.zip** qui, lorsqu'extrait, contient l'exécutable du jeu ainsi que tous ces fichiers de ressource.

Un exemple de fichier *.conj* est disponible dans le dossier *Docs/Demo*.