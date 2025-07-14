using System.Text;

namespace CourseVoiture;

public partial class Course
{
    // ********************************************* //
    // ******* METHODES SANS CLAVIER NI ECRAN ****** //
    // ********************************************* //
    // ******** A vérifier en tests unitaires ****** //
    // ********************************************* //

        /// <summary>
    /// Initialise le jeu avec le parcours par défaut est utilisé
    /// A vérifier en tests unitaires        
    /// </summary>    
    public Course()
    {
        parcours = new string[HauteurJeu];
        for (int i = 0; i < HauteurJeu; i++)
        {
            var sb = new StringBuilder();
            for (int j = 0; j < LargeurJeu; j++)
            {
                // Murs aux bords de la route
                int bordGauche = (LargeurJeu - LargeurRoute) / 2;
                int bordDroit  = bordGauche + LargeurRoute - 1;

                if (j == bordGauche || j == bordDroit)
                    sb.Append(MurParcours);    // Ajoute un mur à la bordure gauche ou droite
                else if (j > bordGauche && j < bordDroit)
                    sb.Append(RouteParcours);  // Ajoute la route à l'intérieur
                else
                    sb.Append(HerbeParcours);  // Ajoute de l'herbe en dehors de la route
            }
            parcours[i] = sb.ToString(); // Construit la ligne complète du parcours
        }
    }

    /// <summary>
    /// Initialise le jeu en lisant le parcours du fichier de parcours.
    /// Si le fichier ne peut être lu, un parcours par défaut est utilisé
    /// en utilisant le constructeur par défaut
    /// A vérifier en tests unitaires 
    /// </summary>
    /// <param name="fichierParcours">Nom du fichier de parcours (de préférence relatif)</param>
    public Course(string fichierParcours) : this()
    {
        LireParcours(fichierParcours); // Tente de charger un parcours à partir d'un fichier
    }

    /// <summary>
    /// Lit un parcours dans un fichier et calcule ses caractèristiques comme la longueur.
    /// En cas d'erreur, charge un parcours par défaut.
    /// </summary>
    /// <param name="fichierParcours">Nom du fichier de parcours (de préférence relatif)</param>
    /// <returns>true si OK, false si le parcours par défaut a dû être chargé</returns>
    public bool LireParcours(string fichierParcours)
    {
        try
        {
            string[] lignes = File.ReadAllLines(fichierParcours);

            if (lignes.Length < HauteurJeu)
                return false; // Fichier trop court

            parcours = new string[HauteurJeu];

            for (int i = 0; i < HauteurJeu; i++)
            {
                if (lignes[i].Length < LargeurJeu)
                    return false; // Ligne trop courte

                parcours[i] = lignes[i][..LargeurJeu]; // Copie la ligne tronquée à la largeur du jeu
            }

            return true; // Chargement réussi
        }
        catch
        {
            return false; // En cas d’erreur, retourne false
        }
    }

    /// <summary>
    /// Charge un parcours au hasard
    /// </summary>
    /// <param name="folderName">Nom du répertoire avec les parcours</param>
    /// <returns>true si OK, false si le parcours par défaut a dû être chargé</returns>
    public bool ChargerParcoursAuHasard(string folderName = "./")
    {
        try
        {
            string[] fichiers = Directory.GetFiles(folderName, "*.txt");

            if (fichiers.Length == 0)
                return false; // Aucun fichier trouvé

            var rand = new Random();
            string fichier = fichiers[rand.Next(fichiers.Length)]; // Choix aléatoire d'un fichier

            return LireParcours(fichier); // Charge le parcours choisi
        }
        catch
        {
            return false; // En cas d’erreur
        }
    }

    /// <summary>
    /// Initialise le terrain du parcours et retient le moment de début 
    /// d'une partie pour mesurer la durée du jeu.
    /// La ligne 0 est en bas de l'écran,
    /// puis les lignes 1 à 5 contiennent la voiture, 
    /// puis les lignes 6 à la fin contiennent le reste du parcours
    /// </summary>
    public void InitialiserPartie()
    {
        vitesseActuelle = VITESSE_INITIALE; // Vitesse de départ
        positionVoiture = (LargeurJeu - LargeurRoute) / 2 + LargeurRoute / 2; // Position centrale de la voiture
        PartieEnCours = true; // Partie active
        debutCalcul = DateTime.Now; // Date/heure du début de partie
        distanceTotale = 0; // Distance parcourue remise à zéro
        nomJoueur = ""; // Nom joueur vide au départ

        if (parcours == null || parcours.Length == 0)
        {
            var parcoursParDefaut = new Course(); // Charge un parcours par défaut si aucun parcours chargé
            parcours = parcoursParDefaut.parcours;
        }
    }

    /// <summary>
    /// Fonction qui fait avancer la route en la décalant vers le bas
    /// et en insérant une nouvelle ligne en haut, qui fait tourner la
    /// route à gauche ou à droite, ou bien continuer tout droit, 
    /// selon le parcours de jeu téléchargé
    /// </summary>
    public void AvancerRoute()
    {
        if (parcours == null || parcours.Length == 0)
        {
            throw new InvalidOperationException("Le parcours n'est pas initialisé.");
        }

        // Convertir parcours en List<string> localement
        var parcoursList = new List<string>(parcours);

        // Retirer la dernière ligne du parcours et l'ajouter en haut
        parcoursList.Insert(0, parcoursList[^1]); // Insère la dernière ligne en première position
        parcoursList.RemoveAt(parcoursList.Count - 1); // Supprime la dernière ligne qui a été dupliquée

        // Mettre à jour le tableau parcours
        parcours = parcoursList.ToArray();
    }

    /// <summary>
    /// Fonction qui fait se déplacer la voiture vers la gauche ou la droite
    /// et vérifie s'il y a une sortie de route, détectée en tant que
    /// collision entre une ou plusieurs roues avec la bordure ou la pelouse
    /// </summary>
    public void DeplacerVoiture()
    {
        // Calcul des bordures dynamiques de la route pour la ligne actuelle
        string ligneActuelle = parcours[HauteurJeu - 1]; // Ligne où se trouve la voiture (bas de l’écran)
        int bordGauche = ligneActuelle.IndexOf('█')+2; // Trouver la première bordure gauche (avec décalage)
        int bordDroit = ligneActuelle.LastIndexOf('█')-4; // Trouver la dernière bordure droite (avec décalage)

        if (directionVoitureEnMemoire.HasValue)
        {
            // Déplacer la voiture vers la gauche ou la droite
            if (directionVoitureEnMemoire == Directions.Droite)
            {
                positionVoiture++;          // Incrémente position pour aller à droite
                toucheDirection = 2;        // Touche indiquant tournant à droite
            }
            else if (directionVoitureEnMemoire == Directions.Gauche)
            {
                positionVoiture--;          // Décrémente position pour aller à gauche
                toucheDirection = 1;        // Touche indiquant tournant à gauche
            }
            directionVoitureEnMemoire = null; // Réinitialise la direction en mémoire
        }

        // Vérifier si la voiture est sortie de la route
        if (positionVoiture <= bordGauche || positionVoiture >= bordDroit)
        {
            // Collision détectée : sortie de route
            PartieEnCours = false; // Arrêter la partie

            // Affichage du message de collision clignotant au centre de la console
            Console.Clear();
            string message = "COLLISION DÉTECTÉE !";
            int screenWidth = Console.WindowWidth;
            int screenHeight = Console.WindowHeight;
            int messageX = (screenWidth - message.Length) / 2;
            int messageY = screenHeight / 2;

            for (int i = 0; i < 10; i++) // Clignotement 10 fois
            {
                Console.Clear();
                Console.BackgroundColor = (i % 2 == 0) ? ConsoleColor.Red : ConsoleColor.Black;
                Console.ForegroundColor = (i % 2 == 0) ? ConsoleColor.White : ConsoleColor.Red;

                Console.SetCursorPosition(messageX, messageY);
                Console.Write(message);

                Thread.Sleep(200); // Pause pour l'effet de clignotement
            }
            return;
        }
    }

    /// <summary>
    /// Fonction pour accélérer, c'est-à-dire augmenter la vitesse
    /// L'accélération dépend du facteur d'accélération du type de partie
    /// et de la vitesse (quand on va vite, on accélère moins)
    /// La vitesse est limitée à VITESSE_MAX
    /// </summary>
    public void Accelerer()
    {
        // Augmentation plus douce (dépend de la vitesse actuelle)
        float facteur = 1f - ((float)vitesseActuelle / VITESSE_MAX); // Facteur diminue avec la vitesse

        int accelerationChoix = 0;

        if (typeDeJeuSelection == TypesDeJeu.Distance)
        {
            accelerationChoix = (int)(ACCELERATION_DISTANCE * facteur); // Accélération spécifique Distance
        }
        else if (typeDeJeuSelection == TypesDeJeu.Chrono)
        {
            // BONUS : accélération spécifique Chrono
            accelerationChoix = (int)(ACCELERATION_CHRONO * facteur);
        }

        int gain = (int)(accelerationChoix * facteur);

        if (gain < 1) gain = 1; // Accélération minimale de 1

        vitesseActuelle += gain; // Augmente la vitesse
        if (vitesseActuelle > VITESSE_MAX)
            vitesseActuelle = VITESSE_MAX; // Limite à VITESSE_MAX

        // Avancée proportionnelle plus contrôlée
        int avancees = (int)Math.Ceiling((double)vitesseActuelle / 50); // Moins brutal

        for (int i = 0; i < avancees; i++)
        {
            // Si on joue en mode Distance et que la limite est atteinte, stoppe la partie
            if (typeDeJeuSelection == TypesDeJeu.Distance && distanceTotale >= DistanceLimit)
            {
                PartieEnCours = false;
                break; // Stoppe la boucle pour éviter de dépasser la limite
            }

            AvancerRoute(); // Fait avancer la route
            distanceTotale++; // Incrémente la distance totale
        }
    }

  
}