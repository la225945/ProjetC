namespace CourseVoiture;

public partial class Course
{
    // ********************************************* //
    // ************** ENUMERATIONS ***************** //
    // ********************************************* //

    // Directions possibles pour les roues de la voiture
    public enum Directions
    {
        Haut,
        Bas,
        Droite,
        Gauche
    }

    Directions? directionVoitureEnMemoire = null; // Direction de la voiture

    // Types de parties (pour le bonus)
    public enum TypesDeJeu
    {
        Distance,
        //BONUS
        Chrono
    }
    TypesDeJeu? typeDeJeuSelection = null; // Type de jeu (Distance ou Chrono)

    // ********************************************* //
    // *************** CONSTANTES ****************** //
    // ********************************************* //
    // Dimensions du jeu et de la route
    public const int LargeurJeu = 70;     // Largeur de l'écran de jeu
    public const int HauteurJeu = 20;     // Hauteur de l'écran de jeu
    public const int LargeurRoute = 26;  // Largeur de la route
    
    // Consantes pour la vitesse est le temps d'attente en ms entre deux mouvements
    public const int VITESSE_INITIALE = 5;       // Vitesse de départ en km/h
    public const int VITESSE_MAX = 100;         // Vitesse maximale en km/h
    public const int TEMPS_ATTENTE_MS = 200;    // Temps d'attente initial entre deux déplacements (en ms)


    // Constantes pour l'accélération
    // Bonus: l'accélération dépend du type de partie
    public const float ACCELERATION_DISTANCE = 2.0f;  // Accélération pour le mode Distance
    //BONUS
    public const float ACCELERATION_CHRONO = 1.5f;   // Accélération pour le mode Chrono (bonus)


    // Constantes pour le dessin de la voiture
    // Lien utile: https://symbl.cc/en/unicode-table/#box-drawing
    // ▐ ▌ ▊ ▀ ▅ ░ ▒ ▓ ╲ ╱ \ / ┝ ┥ ┣ ┫ |
    public const string VoitureHaut = "◢ ◣";  // Avant de la voiture
    public const string VoitureRoue = "┝█████┥";  // Roue de la voiture
    public const string VoitureRoueTourneGauche = "╱█████╱";  // Roue de la voiture
    public const string VoitureRoueTourneDroite = "╲█████╲";  // Roue de la voiture
    public const string VoitureMoteur = "███"; // Moteur de la voiture
    public const string VoiturePilote = "▌ô▐";  // Habitacle du pilot


    // Constantes pour le dessin du terrain
    // ░ ▒ ▓ █  (0x2591 à 0x2594) donnent un niveau d'intensité de █ sous Linux
    public const string HerbeParcours = "░";  // Représentation de l'herbe
    public const string MurParcours = "█";    // Représentation des murs
    public const string MilieuRouteParcours = "▓"; // Marquage central de la route
    public const string RouteParcours = " ";  // Représentation de la route



    // ********************************************* //
    // *************** PROPRIETES ****************** //
    // ********************************************* //
    // Parcours
    // A REMPLIR


    // Couleurs du terrain et de la voiture. La route a la coudeur du fond du décor
    public const ConsoleColor CouleurVoiture = ConsoleColor.Red;   // Couleur de la voiture
    public const ConsoleColor CouleurRoute = ConsoleColor.Black;    // Couleur de la route
    public const ConsoleColor CouleurHerbe = ConsoleColor.Green;   // Couleur de l'herbe
    public const ConsoleColor CouleurMur = ConsoleColor.DarkGray;  // Couleur des murs

    // Parcours 
    public string[] parcours;

    // Mesure du temps pour le jeu de durée
    

    // Mesure du temps de calcul
    public DateTime debutCalcul;
    public int dureeCalcul;

    // Paramètres de la course
    public int vitesseActuelle = VITESSE_INITIALE;
    public int positionVoiture = (LargeurJeu - LargeurRoute) / 2 + LargeurRoute / 2;
    public int distanceTotale = 0;
    public string nomJoueur = null;
    const int DistanceLimit = 1000;

    //Ajout
    //Constante pour le toûche de contrôles
    public HashSet<ConsoleKey> touchesEnfoncees = new();
    // Constante pour une touche null
    public int toucheDirection = 0;

    //Ajout
    // Constantes pour les couleurs
    public const ConsoleColor CouleurFond = ConsoleColor.Black;      // Couleur de fond de l'écran
    public const ConsoleColor CouleurTexte = ConsoleColor.White;    // Couleur du texte
    public const ConsoleColor CouleurTitre = ConsoleColor.Blue;     // Couleur des titres   


    public bool PartieEnCours = false;
}