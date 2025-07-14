using System.Collections.Generic;
using MySqlConnector;

namespace CourseVoiture;

public partial class Course
{
    /// <summary>
    /// Structure pour stocker une partie (nom, distance, durée, type de jeu si bonus)
    /// </summary>
    public struct ScorePartie()
    {
        public string nomJoueur;
        public float DistancePartie;
        public float Duree;
        public TypesDeJeu JeuPartie;

        public override string ToString()
        {
            // Retourne une chaîne résumant le score
            return $"{nomJoueur} - Distance : {DistancePartie} m | Durée : {Duree / 1000:F2} s | Mode : {JeuPartie}";
        }
    }

    /// <summary>
    /// Efface la base de données
    /// </summary>
    /// <param name="DBDeTest">Indique si c'est la DB de test ou de production</param>
    /// <param name="messageDerreur">Pour renvoyer un texte explicatif s'il y a un erreur</param>
    /// <returns>Renvoie true si OK, false s'il y a une erreur</returns>
    public static bool EffacerDB(bool DBDeTest, out string messageDerreur)
    {
        // Choix du nom de base selon test ou prod
        string dbName;
        if (DBDeTest)
        {
            dbName = "la225945";
        }
        else
        {
            dbName = "la225945_test";
        }

        messageDerreur = string.Empty;

        try
        {
            // Connexion au serveur MySQL sans base sélectionnée
            string serverConn = "Server=localhost;Uid=root;Pwd=;";
            using var connServer = new MySqlConnection(serverConn);
            connServer.Open();

            // Exécute la suppression de la base si elle existe
            using var cmdDb = connServer.CreateCommand();
            cmdDb.CommandText = $"DROP DATABASE IF EXISTS `{dbName}`;";
            cmdDb.ExecuteNonQuery();

            return true;
        }
        catch (Exception ex)
        {
            // Capture message d’erreur en cas d’échec
            messageDerreur = ex.Message;
            return false;
        }
    }

    /// <summary>
    /// Créer la base de donnée
    /// </summary>
    /// <param name="DBDeTest">Indique si c'est la DB de test ou de production</param>
    /// <param name="messageDerreur">Pour renvoyer un texte explicatif s'il y a un erreur</param>
    /// <returns>Renvoie true si OK, false s'il y a une erreur</returns>
    public static bool CreerDB(bool DBDeTest, out string messageDerreur)
    {
        // Choix du nom de base selon test ou prod
        string dbName;
        if (DBDeTest)
        {
            dbName = "la225945";
        }
        else
        {
            dbName = "la225945_test";
        }

        messageDerreur = string.Empty;

        try
        {
            // Connexion serveur MySQL
            string serverConn = "Server=localhost;Uid=root;Pwd=;";
            using var connServer = new MySqlConnection(serverConn);
            connServer.Open();

            // Crée la base si absente
            using var cmdDb = connServer.CreateCommand();
            cmdDb.CommandText = $"CREATE DATABASE IF NOT EXISTS `{dbName}`;";
            cmdDb.ExecuteNonQuery();

            // Connexion à la base créée
            string connStr = $"Server=localhost;Database={dbName};Uid=root;Pwd=;";
            using var conn = new MySqlConnection(connStr);
            conn.Open();

            // Crée les tables joueurs et scores avec clés primaires et étrangères
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS joueurs(
                    id INT AUTO_INCREMENT PRIMARY KEY,
                    nom VARCHAR(50) UNIQUE NOT NULL
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
                CREATE TABLE IF NOT EXISTS scores(
                    id INT AUTO_INCREMENT PRIMARY KEY,
                    joueur_id INT NOT NULL,
                    distance FLOAT NOT NULL,
                    duree FLOAT NOT NULL,
                    type_jeu VARCHAR(50) NOT NULL,
                    FOREIGN KEY (joueur_id) REFERENCES joueurs(id) ON DELETE CASCADE
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;";
            cmd.ExecuteNonQuery();

            return true;
        }
        catch (Exception ex)
        {
            // Capture message d’erreur en cas d’échec
            messageDerreur = ex.Message;
            return false;
        }
    }

    /// <summary>
    /// Renvoie d'identifiant d'un joueur 
    /// </summary>
    /// <param name="connexion">Connexion à la DB</param>
    /// <param name="nom">Nom du joueur</param>
    /// <returns>Renvoie l'ID du joueur (clé primaire)</returns>
    private static int LireIDJoueur(MySqlConnection connexion, string nom)
    {
        using var cmd = new MySqlCommand("SELECT id FROM joueurs WHERE nom=@nom", connexion);
        cmd.Parameters.AddWithValue("@nom", nom);
        var result = cmd.ExecuteScalar();
        if (result != null)
            return Convert.ToInt32(result);

        // Si aucun joueur n'est trouvé, retourne -1
        return -1;
    }

    /// <summary>
    /// Ajoute un score à la base de données
    /// </summary>
    /// <param name="DBDeTest">Indique si c'est la DB de test ou de production</param>
    /// <param name="nom">Nom du joueur</param>
    /// <param name="distance">Distance parcourue en m</param>
    /// <param name="duree">Durée en millisecondes</param>
    /// <param name="typeDeJeu">Type de jeu (pour le bonus uniquement)</param>
    /// <param name="messageDerreur">Pour renvoyer un texte explicatif s'il y a un erreur</param>
    /// <returns>Renvoie true si OK, false s'il y a une erreur</returns>
    public static bool AjouterScore(bool DBDeTest, string nom, int distance, int duree, TypesDeJeu typeDeJeu, out string messageDerreur)
    {
        // Choix du nom de base selon test ou prod
        string dbName;
        if (DBDeTest)
        {
            dbName = "la225945";
        }
        else
        {
            dbName = "la225945_test";
        }
        messageDerreur = "";
        string connStr = $"Server=localhost;Database={dbName};Uid=root;Pwd=;";
        try
        {
            using var conn = new MySqlConnection(connStr);
            try { conn.Open(); }
            catch (MySqlException ex) when (ex.Number == 1049) // Base inconnue
            {
                if (!CreerDB(DBDeTest, out messageDerreur))
                    return false; // Création échouée
                conn.ConnectionString = connStr;
                conn.Open();
            }

            // Ajoute ou récupère le joueur selon nom
            using (var cmdJ = conn.CreateCommand())
            {
                cmdJ.CommandText = @"
                    INSERT INTO joueurs(nom)
                    VALUES (@nom)
                    ON DUPLICATE KEY UPDATE id=LAST_INSERT_ID(id);";
                cmdJ.Parameters.AddWithValue("@nom", nom);
                cmdJ.ExecuteNonQuery();
            }

            // Récupère l'ID inséré ou existant
            int idJoueur;
            using (var cmdId = conn.CreateCommand())
            {
                cmdId.CommandText = "SELECT LAST_INSERT_ID();";
                idJoueur = Convert.ToInt32(cmdId.ExecuteScalar());
            }

            // Insère le score lié à ce joueur
            using var cmdS = conn.CreateCommand();
            cmdS.CommandText = @"
                INSERT INTO scores(joueur_id, distance, duree, type_jeu)
                VALUES(@jid, @dist, @dur, @type);";
            cmdS.Parameters.AddWithValue("@jid", idJoueur);
            cmdS.Parameters.AddWithValue("@dist", distance);
            cmdS.Parameters.AddWithValue("@dur", duree);
            cmdS.Parameters.AddWithValue("@type", typeDeJeu.ToString());
            cmdS.ExecuteNonQuery();

            return true;
        }
        catch (Exception ex)
        {
            // Capture message d’erreur en cas d’échec
            messageDerreur = ex.Message;
            return false;
        }
    }

    /// <summary>
    /// Renvoie une liste de meilleurs scores
    /// </summary>
    /// <param name="DBDeTest">Indique si c'est la DB de test ou de production</param>
    /// <param name="nombreDeScores">Nombre de scores maximum à renvoyer</param>
    /// <param name="typeDeJeu">Type de jeu (pour le bonus uniquement)</param>
    /// <param name="messageDerreur">Pour renvoyer un texte explicatif s'il y a un erreur</param>
    /// <returns>Une Liste typée de structures ScorePartie</returns>
    public static List<ScorePartie>? LireScores(bool DBDeTest, int nombreDeScores, TypesDeJeu typeDeJeu, out string messageDerreur)
    {
        // Choix du nom de base selon test ou prod
        string dbName;
        if (DBDeTest)
        {
            dbName = "la225945";
        }
        else
        {
            dbName = "la225945_test";
        }   

        messageDerreur = string.Empty;

        string connStr = $"Server=localhost;Database={dbName};Uid=root;Pwd=;";
        try
        {
            using var conn = new MySqlConnection(connStr);
            conn.Open();

            // Choix du tri selon type de jeu : distance ou durée
            string ordrePartype = typeDeJeu == TypesDeJeu.Distance ? "s.distance DESC" : "s.duree DESC";

            using var cmd = new MySqlCommand(
                $"SELECT j.nom, s.distance, s.duree, s.type_jeu FROM scores s JOIN joueurs j ON s.joueur_id = j.id " +
                $"WHERE s.type_jeu = @type ORDER BY {ordrePartype} LIMIT @n", conn);

            cmd.Parameters.AddWithValue("@type", typeDeJeu.ToString());
            cmd.Parameters.AddWithValue("@n", nombreDeScores);

            using var reader = cmd.ExecuteReader();
            var list = new List<ScorePartie>();
            while (reader.Read())
            {
                // Lecture des résultats et création des structures ScorePartie
                list.Add(new ScorePartie
                {
                    nomJoueur = reader.GetString(0),
                    DistancePartie = reader.GetFloat(1),
                    Duree = reader.GetFloat(2),
                    JeuPartie = Enum.Parse<TypesDeJeu>(reader.GetString(3))
                });
            }
            return list;
        }
        catch (Exception ex)
        {
            // Capture message d’erreur en cas d’échec
            messageDerreur = ex.Message;
            return null;
        }
    }

}
