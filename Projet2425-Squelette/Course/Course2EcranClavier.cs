using System.Text;

namespace CourseVoiture;

public partial class Course
{
    // ********************************************* //
    // **** METHODES POUR L'AFFICHAGE A L'ECRAN **** //
    // ********************************************* //
    /// Initialisation du jeu: dimensions de la console
    /// Ne pas utiliser en tests unitaires
    /// Jouer une partie comporte plusieurs phases
    /// - Initialiser
    /// - Boucler tant que la partie est en cours
    /// - Terminer avec un dialogue <summary>


    /// <summary>
    /// Initialisation du jeu en fonction de la console
    /// </summary>
    /// <param name="ModeTest">true c'est pendant les tests</param>
    public void InitialiserJeu(bool ModeTest = false)
    {
        Console.Clear();
        Console.SetWindowSize(LargeurJeu, HauteurJeu); // On initialise la taille de l'écran

        Console.CursorVisible = false; // On cache le curseur
        Console.BackgroundColor = CouleurFond; // On met le fond en noir
        Console.ForegroundColor = CouleurTexte; // On met le texte en blanc
    }

    /// <summary>
    /// Dialogue de fin de jeu. 
    /// Remet les couleurs aux valeurs par défaut, efface l'écran et affiche un message d'au revoir
    /// Ne pas utiliser en tests unitaires
    /// </summary>
    public void TerminerJeu()
    {
        Console.ResetColor(); // On remet les couleurs par défaut
        Console.Clear(); // On efface l'écran
        Console.SetCursorPosition(0, 0); // On remet le curseur en haut à gauche
        Console.WriteLine("Merci d'avoir joué à la Course !");
        Console.WriteLine("A bientôt !");
    }

    /// <summary>
    /// Affichage des meilleurs scores
    /// Ne pas utiliser en tests unitaires
    /// </summary>  
    public void AfficherScores()
    {
        Console.Clear();
        // Affichage de l'en-tête encadré
        Console.WriteLine("\t\t\t╓────────────────────────╖");
        Console.WriteLine("\t\t\t║    MEILLEURS SCORES    ║");
        Console.WriteLine("\t\t\t╙────────────────────────╜");

        // Lecture des meilleurs scores pour le mode Distance
        var scores = LireScores(
            DBDeTest: true, 5, TypesDeJeu.Distance, out string messageErreur
        );

        if (!string.IsNullOrEmpty(messageErreur))
        {
            Console.WriteLine($"Erreur : {messageErreur}");
        }
        else if (scores == null)
        {
            Console.WriteLine("Aucun score n'a pu être récupéré.");
        }
        else
        {
            Console.WriteLine("\nListe des meilleurs scores distance :\n");
            foreach (var score in scores)
            {
                Console.WriteLine(score); // Appelle automatiquement ToString()
            }
        }

        // Lecture des meilleurs scores pour le mode Chrono
        var scoresChrono = LireScores( 
            DBDeTest: true, 5, TypesDeJeu.Chrono, out string messageErreurChrono
        );
        if (!string.IsNullOrEmpty(messageErreur))
        {
            Console.WriteLine($"Erreur : {messageErreur}");
        }
        else if (scoresChrono == null)
        {
            Console.WriteLine("Aucun score n'a pu être récupéré.");
        }
        else
        {
            Console.WriteLine("\nListe des meilleurs scores chrono :\n");
            foreach (var score in scoresChrono)
            {
                Console.WriteLine(score); // Appelle automatiquement ToString()
            }
        }

        Console.WriteLine("Appuyez sur une touche pour revenir au menu...");
        Console.ReadKey();
    }

    /// <summary>
    /// Dialogue principal
    /// Affiche le menu principal et l'aide
    /// En cas de bonus, fait choisir entre les types de jeu: Distance ou Temps 
    /// Vérifie la taille de l'écran
    /// Ne pas utiliser en tests unitaires
    /// </summary>
    /// <returns>true s'il faut jouer une partie, false s'il faut quitter le jeu. 
    /// Dans les autres case, on reste dans le menu et des sous-fonctions</returns>
    public bool MenuPrincipal()
    {
        bool choixValide = false;

        while (!choixValide)
        {
            Console.Clear();
            Console.WriteLine("\t\t\t╓─────────────────────╖");
            Console.WriteLine("\t\t\t║       COURSE        ║");
            Console.WriteLine("\t\t\t╙─────────────────────╜");
            // Affichage du menu
            Console.WriteLine("\n\t\t\t     Menu Principal\n");

            // Affichage de la voiture (dessin symbolique)
            Console.WriteLine("\t\t\t\t  "+ VoitureHaut);
            Console.WriteLine("\t\t\t\t"+VoitureRoue);
            Console.WriteLine("\t\t\t\t  "+VoitureMoteur);
            Console.WriteLine("\t\t\t\t  "+VoiturePilote);
            Console.WriteLine("\t\t\t\t"+VoitureRoue+"\n");

            // Options du menu
            Console.WriteLine("\t\t 1. Jouer au mode Distance");
            Console.WriteLine("\t\t 2. Jouer au mode Chrono");
            Console.WriteLine("\t\t 3. Aide");
            Console.WriteLine("\t\t 4. Scores");
            Console.WriteLine("\t\t 5. Quitter");

            // Lecture de l'entrée utilisateur
            ConsoleKeyInfo touche = Console.ReadKey(true);

            switch (touche.KeyChar)
            {
                case '1':
                    Console.Clear();
                    Console.WriteLine("Mode Distance sélectionné !");
                    typeDeJeuSelection = TypesDeJeu.Distance;
                    Thread.Sleep(2000);
                    return true; // Lancer une partie
                case '2':
                    Console.Clear();
                    Console.WriteLine("Mode Chrono sélectionné !");
                    typeDeJeuSelection = TypesDeJeu.Chrono;
                    Thread.Sleep(1000);
                    return true; // Lancer une partie (bonus)

                case '3':
                    AfficherAide(); // L'utilisateur lit l'aide et revient en appuyant sur Entrée
                    break;

                case '4':
                    AfficherScores(); // L'utilisateur lit les scores et revient
                    break;

                case '5':
                    TerminerJeu();
                    return false; // Quitter le jeu

                default:
                    Console.WriteLine("\nChoix invalide, veuillez appuyer sur une touche pour réessayer.");
                    Console.ReadKey(true); // Attente avant retour au menu
                    break;
            }
        }

        return false; // Par sécurité, même si ce point ne devrait jamais être atteint
    }

    /// <summary>
    /// Dessine le circuit à l'écran
    /// Elle dessine des lignes de caractères représentant l'herbe et la route
    /// sauf en haut au milieu, où elle affiche les données de vitesse, distance et durée
    /// et en bas où elle dessine la voiture avec les roues qui montrent la direction suivie
    /// A ne pas utiliser en tests unitaires
    /// </summary>
    public void DessinerParcours()
    {
        Console.Clear();

        // Largeur des bords (herbe + murs) de chaque côté
        int bordGauche = (LargeurJeu - LargeurRoute) / 2;
        int bordDroit = bordGauche + LargeurRoute - 1;

        // Dessiner le parcours ligne par ligne
        for (int ligne = 0; ligne < parcours.Length; ligne++)
        {
            Console.SetCursorPosition(0, ligne);

            // Construire la ligne du parcours
            string ligneParcours = parcours[ligne];

            if (ligne == 0) // Ligne des informations : cadre supérieur
            {
                // Remplacer la partie centrale par le haut du cadre
                int positionInfo = bordGauche + (LargeurRoute / 2) - 10; // Centrer le cadre
                ligneParcours = ligneParcours.Substring(0, positionInfo) +
                                "╔════════════════════╗" +
                                ligneParcours.Substring(positionInfo + 20);
            }
            else if (ligne == 1) // Ligne des informations : vitesse
            {
                // Remplacer la partie centrale par la ligne de vitesse
                int positionInfo = bordGauche + (LargeurRoute / 2) - 10; // Centrer les infos
                string info = $"║ Vitesse : {vitesseActuelle} km/h  ║";
                ligneParcours = ligneParcours.Substring(0, positionInfo) +
                                info +
                                ligneParcours.Substring(positionInfo + info.Length);
            }
            else if (ligne == 2) // Ligne des informations : distance
            {
                // Remplacer la partie centrale par la ligne de distance
                int positionInfo = bordGauche + (LargeurRoute / 2) - 10; // Centrer les infos
                string info = $"║ Distance : {distanceTotale} m   ║";
                ligneParcours = ligneParcours.Substring(0, positionInfo) +
                                info +
                                ligneParcours.Substring(positionInfo + info.Length);
            }
            else if (ligne == 3) // Ligne des informations : temps
            {
                // Remplacer la partie centrale par la ligne de temps
                int positionInfo = bordGauche + (LargeurRoute / 2) - 10; // Centrer les infos
                TimeSpan tempsDeJeu = DateTime.Now - debutCalcul;
                string info = $"║ Temps : {tempsDeJeu:mm\\:ss}      ║";
                ligneParcours = ligneParcours.Substring(0, positionInfo) +
                                info +
                                ligneParcours.Substring(positionInfo + info.Length);
            }
            else if (ligne == 4) // Ligne de fermeture du cadre
            {
                // Remplacer la partie centrale par le bas du cadre
                int positionInfo = bordGauche + (LargeurRoute / 2) - 10; // Centrer le cadre
                ligneParcours = ligneParcours.Substring(0, positionInfo) +
                                "╚════════════════════╝" +
                                ligneParcours.Substring(positionInfo + 20);
            }
            else if (ligne >= HauteurJeu - 5 && ligne < HauteurJeu) // Lignes de la voiture
            {
                switch (ligne - (HauteurJeu - 5)) // Ajuster l'indice pour correspondre aux 5 dernières lignes
                {
                    case 0:
                        // Haut de la voiture
                        Console.Write(parcours[ligne].Substring(0, positionVoiture + 2 - 1)); // Décalage de 2
                        Console.ForegroundColor = CouleurTitre;
                        Console.Write(VoitureHaut);
                        Console.ForegroundColor = CouleurTexte;
                        Console.WriteLine(parcours[ligne].Substring(positionVoiture + 2 + 1)); // Décalage de 2
                        break;
                    case 1:
                        // Roues centrales
                        Console.Write(parcours[ligne].Substring(0, positionVoiture + 2 - 3)); // Décalage de 2
                        Console.ForegroundColor = CouleurTitre;
                        if (toucheDirection == 2)
                        {
                            // Roue avant gauche (tournée à gauche)
                            Console.Write(VoitureRoueTourneGauche);
                        }
                        else if (toucheDirection == 1)
                        {
                            // Roue avant droite (tournée à droite)
                            Console.Write(VoitureRoueTourneDroite);
                        }
                        else if (toucheDirection == 3)
                        {
                            // Roue avant droite (droite tout droit)
                            Console.Write(VoitureRoue);
                        }
                        Console.ForegroundColor = CouleurTexte;
                        Console.WriteLine(parcours[ligne].Substring(positionVoiture + 2 + 3)); // Décalage de 2
                        break;
                    case 2:
                        // Moteur
                        Console.Write(parcours[ligne].Substring(0, positionVoiture + 2 - 1)); // Décalage de 2
                        Console.ForegroundColor = CouleurTitre;
                        Console.Write(VoitureMoteur);
                        Console.ForegroundColor = CouleurTexte;
                        Console.WriteLine(parcours[ligne].Substring(positionVoiture + 2 + 1)); // Décalage de 2
                        break;
                    case 3:
                        // Pilote
                        Console.Write(parcours[ligne].Substring(0, positionVoiture + 2 - 1)); // Décalage de 2
                        Console.ForegroundColor = CouleurTitre;
                        Console.Write(VoiturePilote);
                        Console.ForegroundColor = CouleurTexte;
                        Console.WriteLine(parcours[ligne].Substring(positionVoiture + 2 + 1)); // Décalage de 2
                        break;
                    case 4:
                        // Roues arrière
                        Console.Write(parcours[ligne].Substring(0, positionVoiture + 2 - 3)); // Décalage de 2
                        Console.ForegroundColor = CouleurTitre;
                        Console.Write(VoitureRoue);
                        Console.ForegroundColor = CouleurTexte;
                        Console.WriteLine(parcours[ligne].Substring(positionVoiture + 2 + 3)); // Décalage de 2
                        break;
                }
            }

            // Afficher la ligne complète
            Console.WriteLine(ligneParcours);
        }
    }

    /// <summary>
    /// Commence le comptage du temps pour calculer et afficher un déplacement
    /// La variable global debutCalcul retient l'heure de départ
    /// Ne pas utiliser en tests unitaires
    /// </summary>
    public void DebutComptageTemps()
    {
        // On retient l'heure de départ
        // On initialise la variable dureeCalcul à 0
        
        DateTime debutCalcul = DateTime.Now;
        dureeCalcul = 0;
        
    }

    /// <summary>
    /// Termine le comptage du temps pour calculer et afficher un déplacement
    /// La variable globale dureeCalcul contient le résultat en ms
    /// Ce temps devra être retiré du sleep simulant la vitesse
    /// de façon à tenir compte du temps de calcul du déplacmene et de l'affichage
    /// Ne pas utiliser en tests unitaires
    /// </summary>
    public void FinComptageTemps()
    {
        // On calcule le temps de calcul
        // On le retire du temps d'attente
        // On affiche le temps de calcul
        
        DateTime finCalcul = DateTime.Now;
        dureeCalcul = (int)(finCalcul - debutCalcul).TotalMilliseconds;
        
    }

    /// <summary>
    /// Fonction qui met en attente le joueur en fonction de la vitesse
    /// Note: sous Windows, l'affichage est plus lent, il faut donc un temps plus court !
    /// Ne pas utiliser en tests unitaires
    /// </summary>
    public void AttendreSelonLaVitesse()
    {
        // On calcule un temps d'attente basé sur la vitesse actuelle
        // Plus la vitesse est grande, plus le délai est petit
        // La formule inverse la vitesse pour créer une décélération naturelle
        int attente = TEMPS_ATTENTE_MS - (vitesseActuelle * 6); 

        // Pour éviter que ça devienne trop rapide ou négatif
        if (attente < 50) attente = 50;

        Thread.Sleep(attente);
    }

    /// <summary>
    /// Affiche un message d'erreur en cas d'erreur à la lecture du parcours
    /// et attend qu'en enfonce ENTER avant de continuer avec un parcours par défaut
    /// Ne pas utiliser en tests unitaires
    /// </summary>
    public void AfficherMessageErreurParcours()
    {
        Console.Clear();
        Console.WriteLine("Erreur de lecture du parcours !");
        Console.WriteLine("Appuyez sur ENTER pour continuer avec un parcours par défaut.");
        Console.ReadKey(true);
        Console.Clear();
        Console.WriteLine("Parcours par défaut chargé.");
        Console.WriteLine("Appuyez sur ENTER pour continuer.");
        Console.ReadKey(true);
        Console.Clear();
    }

    /// <summary>
    /// Affiche le dialogue de fin de partie qui propose une nouvelle partie ou la fin du jeu
    /// Renvoie true en cas de nouvelle partie, false si fin de jeu
    /// A ne pas utiliser en tests unitaires
    /// </summary>
    public void DialogueDeFinDePartie() 
    {
        Console.Clear();

        // Vérifier si la partie est une victoire ou une défaite
        if (distanceTotale >= DistanceLimit && typeDeJeuSelection == TypesDeJeu.Distance)
        {
            Console.WriteLine("Félicitations ! Vous avez remporté la partie en atteignant 1000 mètres !");
            TimeSpan tempsDeJeu = DateTime.Now - debutCalcul;
            Console.WriteLine($"Vous avez joué : {tempsDeJeu:mm\\:ss}");
        }
        else if (distanceTotale < DistanceLimit && typeDeJeuSelection == TypesDeJeu.Distance)
        {
            Console.WriteLine("Défaite ! Vous n'avez pas atteint les 1000 mètres. Essayez encore !");
        }
        else
        {
            TimeSpan tempsDeJeu = DateTime.Now - debutCalcul;
            Console.WriteLine($"Vous avez joué : {tempsDeJeu:mm\\:ss}");
        }

        Console.WriteLine("Quel est votre pseudo ?");
        nomJoueur = Console.ReadLine();

        string messageDerreur;

        if (typeDeJeuSelection == TypesDeJeu.Chrono)
        {
            // Ajouter le score à la base de données
            bool ajoutReussi = AjouterScore(
                DBDeTest: true,
                nom: nomJoueur,
                distance: distanceTotale,
                duree: dureeCalcul,
                typeDeJeu: TypesDeJeu.Chrono, // Vous pouvez ajuster selon le mode de jeu
                out messageDerreur
            );

            if (ajoutReussi)
            {
                Console.WriteLine($"Merci {nomJoueur} d'avoir joué ! Votre score a été enregistré.");
            }
            else
            {
                Console.WriteLine($"Merci {nomJoueur} d'avoir joué ! Une erreur est survenue lors de l'enregistrement du score : {messageDerreur}");
            }
        }
        else
        {
            // Ajouter le score à la base de données
            bool ajoutReussi = AjouterScore(
                DBDeTest: true,
                nom: nomJoueur,
                distance: distanceTotale,
                duree: dureeCalcul,
                typeDeJeu: TypesDeJeu.Distance, // Vous pouvez ajuster selon le mode de jeu
                out messageDerreur
            );

            if (ajoutReussi)
            {
                Console.WriteLine($"Merci {nomJoueur} d'avoir joué ! Votre score a été enregistré.");
            }
            else
            {
                Console.WriteLine($"Merci {nomJoueur} d'avoir joué ! Une erreur est survenue lors de l'enregistrement du score : {messageDerreur}");
            }
        }

        Console.WriteLine("Voulez-vous jouer une nouvelle partie ? (O/N)");

        ConsoleKeyInfo touche;

        // On boucle tant qu'on ne reçoit pas une touche valide
        do
        {
            touche = Console.ReadKey(true);
        }
        while (touche.KeyChar != 'O' && touche.KeyChar != 'o' &&
            touche.KeyChar != 'N' && touche.KeyChar != 'n');

        if (touche.KeyChar == 'O' || touche.KeyChar == 'o')
        {
            InitialiserPartie();
            PartieEnCours = true;
        }
        else
        {
            Console.WriteLine("Merci d'avoir joué !");
            PartieEnCours = false;
        }
    }

    /// <summary>
    /// Lit s'il y a une entrée au clavier et applique le changement correspondant
    /// Attention: si une touche est maintenue enfoncée, elle n'est lue qu'une fois
    /// A ne pas utiliser en tests unitaires
    /// </summary>
    public void LireDeplacementAuClavier()
    {
        while (Console.KeyAvailable)
        {
            ConsoleKeyInfo touche = Console.ReadKey(true);

            // Si la touche est déjà enfoncée, on ignore (évite répétition)
            if (touchesEnfoncees.Contains(touche.Key))
                return;

            // Marquer la touche comme enfoncée
            touchesEnfoncees.Add(touche.Key);

            Directions? direction = null;

            switch (touche.Key)
            {
                case ConsoleKey.Q:
                    direction = Directions.Gauche;
                    directionVoitureEnMemoire = Directions.Gauche;
                    break;
                case ConsoleKey.D:
                    direction = Directions.Droite;
                    directionVoitureEnMemoire = Directions.Droite;
                    break;
                case ConsoleKey.S:
                    direction = Directions.Bas;
                    break;
                case ConsoleKey.Z:
                    direction = Directions.Haut;
                    break;
                case ConsoleKey.Escape:
                    PartieEnCours = false;
                    return;
            }

            if (direction.HasValue)
            {
                switch (direction.Value)
                {
                    case Directions.Haut:
                        Accelerer();
                        toucheDirection = 3; // Touche pour avancer
                        break;
                    case Directions.Bas:
                        vitesseActuelle -= 5;
                        if (vitesseActuelle < VITESSE_INITIALE)
                            vitesseActuelle = VITESSE_INITIALE;
                        break;
                }
            }
        }

        // Quand aucune touche n'est enfoncée, on libère le blocage
        if (!Console.KeyAvailable)
        {
            touchesEnfoncees.Clear();
        }
    }

    public void AfficherAide()
    {
        Console.Clear();
        Console.WriteLine("\t\t\t╓─────────────────────╖");
        Console.WriteLine("\t\t\t║        AIDE         ║");
        Console.WriteLine("\t\t\t╙─────────────────────╜");
        Console.WriteLine("\n\t\t\tContrôles :\n");
        Console.WriteLine("\tZ : Avancer\n \tQ : Gauche\n \tS : Reculer\n \tD : Droite");
        Console.WriteLine("\t\t\tObjectif du jeu : \n\tEviter les murs et parcourir le plus de distance possible.\n \tSi vous heurtez un mur, la partie est terminée.");
        // Bonus
        Console.WriteLine("Bonus : Si vous avez choisi le mode Chrono, vous devez rouler le plus longtemps possible sans vous crasher.\n");
        Console.WriteLine("Scores : Affiche les 5 meilleurs scores enregistrés dans la base de données.\n");

        Console.WriteLine("Appuyez sur ENTER pour revenir au menu...");
        Console.ReadKey(true);
        // Attente jusqu'à ce que l'utilisateur appuie sur Entrée
        while (Console.ReadKey(true).Key != ConsoleKey.Enter) { }
    }


}