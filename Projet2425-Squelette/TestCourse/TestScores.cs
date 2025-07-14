using CourseVoiture;

namespace TestCourseVoiture;

/// <summary>
/// Tests relatifs aux scores:
/// - Création de la base de données
/// - Écriture des joueurs et des scores dans la base de données
/// - Lecture des joueurs et des scores de la base de données
/// Ces tests sont exécutés avec une base de données de test qui est différente 
/// de la base de donnée du jeu (pour ne pas perturber ou perdre les résultats des jeux)
/// </summary>
[TestClass]
[DoNotParallelize]
public class TestScores
{
    private const bool DB_TEST = false;
    private const Course.TypesDeJeu typeDeJeu = Course.TypesDeJeu.Distance;
    private const Course.TypesDeJeu typeDeJeuChrono = Course.TypesDeJeu.Chrono;

    [TestInitialize]
    public void Init()
    {
        Course.EffacerDB(DB_TEST, out _);
        Course.CreerDB(DB_TEST, out _);
    }

    // Teste l'ajout d'un score puis la lecture de ce score
    [TestMethod]
    public void AjouterScore_PuisLireScores_RetourneLeScoreEnregistré()
    {
        string err;
        bool ok = Course.AjouterScore(DB_TEST, "Alice", 500, 1200, typeDeJeu, out err);
        Assert.IsTrue(ok, "AjouterScore doit réussir");

        var scores = Course.LireScores(DB_TEST, 5, typeDeJeu, out err);
        Assert.IsNotNull(scores, "LireScores ne doit pas retourner null");
        Assert.IsTrue(scores.Count >= 1, "Au moins un score doit être présent");

        var first = scores[0];
        Assert.AreEqual("Alice", first.nomJoueur);
        Assert.AreEqual(500f, first.DistancePartie);
        Assert.AreEqual(1200f, first.Duree);
        Assert.AreEqual(typeDeJeu, first.JeuPartie);
    }

    // Teste l'ajout de plusieurs scores pour un même joueur et leur lecture
    [TestMethod]
    public void AjouterScore_MemeJoueur_Multiscores_LisTous()
    {
        Course.AjouterScore(DB_TEST, "Bob", 100, 500, typeDeJeu, out _);
        Course.AjouterScore(DB_TEST, "Bob", 200, 300, typeDeJeu, out _);

        var scores = Course.LireScores(DB_TEST, 5, typeDeJeu, out _);
        Assert.AreEqual(2, scores.Count, "Deux scores pour Bob doivent être lus");
        Assert.IsTrue(scores[0].DistancePartie >= scores[1].DistancePartie,
            "Scores doivent être triés par distance décroissante");
    }

    // Teste la lecture des scores selon différents types de jeu
    [TestMethod]
    public void TestLireScores_TypeDeJeuDifferent()
    {
        Course.AjouterScore(DB_TEST, "Claire", 300, 1500, Course.TypesDeJeu.Distance, out _);
        Course.AjouterScore(DB_TEST, "Dave", 200, 2000, Course.TypesDeJeu.Chrono, out _);

        var scoresDistance = Course.LireScores(DB_TEST, 5, Course.TypesDeJeu.Distance, out _);
        Assert.AreEqual(1, scoresDistance.Count);
        Assert.AreEqual("Claire", scoresDistance[0].nomJoueur);

        var scoresDuree = Course.LireScores(DB_TEST, 5, Course.TypesDeJeu.Chrono, out _);
        Assert.AreEqual(1, scoresDuree.Count);
        Assert.AreEqual("Dave", scoresDuree[0].nomJoueur);
    }

    // Teste que les scores sont triés par distance décroissante
    [TestMethod]
    public void TestLireScores_OrdreDistanceDescendant()
    {
        Course.AjouterScore(DB_TEST, "Eve", 500, 1000, Course.TypesDeJeu.Distance, out _);
        Course.AjouterScore(DB_TEST, "Frank", 700, 1200, Course.TypesDeJeu.Distance, out _);
        Course.AjouterScore(DB_TEST, "Grace", 600, 1100, Course.TypesDeJeu.Distance, out _);

        var scores = Course.LireScores(DB_TEST, 3, Course.TypesDeJeu.Distance, out _);

        Assert.AreEqual(3, scores.Count);
        Assert.AreEqual(700f, scores[0].DistancePartie);
        Assert.AreEqual(600f, scores[1].DistancePartie);
        Assert.AreEqual(500f, scores[2].DistancePartie);
    }

    // Teste que les scores sont triés par durée décroissante
    [TestMethod]
    public void TestLireScores_OrdreDureeDescendant()
    {
        Course.AjouterScore(DB_TEST, "Henry", 100, 2000, Course.TypesDeJeu.Chrono, out _);
        Course.AjouterScore(DB_TEST, "Ivy", 100, 3000, Course.TypesDeJeu.Chrono, out _);
        Course.AjouterScore(DB_TEST, "Jack", 100, 2500, Course.TypesDeJeu.Chrono, out _);

        var scores = Course.LireScores(DB_TEST, 3, Course.TypesDeJeu.Chrono, out _);

        Assert.AreEqual(3, scores.Count);
        Assert.AreEqual(3000f, scores[0].Duree);
        Assert.AreEqual(2500f, scores[1].Duree);
        Assert.AreEqual(2000f, scores[2].Duree);
    }

    // Teste l'ajout d'un score avec caractères spéciaux dans le nom
    [TestMethod]
    public void TestAjouterScore_CaracteresSpeciaux()
    {
        string nomSpecial = "O'Connor; DROP TABLE joueurs;--";
        bool ok = Course.AjouterScore(DB_TEST, nomSpecial, 100, 500, Course.TypesDeJeu.Distance, out string err);

        Assert.IsTrue(ok, "Doit accepter les noms avec caractères spéciaux");

        var scores = Course.LireScores(DB_TEST, 1, Course.TypesDeJeu.Distance, out _);
        Assert.AreEqual(nomSpecial, scores[0].nomJoueur);
    }

    // Teste la limitation du nombre de scores lus
    [TestMethod]
    public void TestLireScores_LimiteNombreDeScores()
    {
        for (int i = 0; i < 10; i++)
        {
            Course.AjouterScore(DB_TEST, $"Player{i}", i * 100, i * 1000, Course.TypesDeJeu.Distance, out _);
        }

        var scores = Course.LireScores(DB_TEST, 5, Course.TypesDeJeu.Distance, out _);

        Assert.AreEqual(5, scores.Count);
        Assert.AreEqual(900f, scores[0].DistancePartie);
        Assert.AreEqual(800f, scores[1].DistancePartie);
        Assert.AreEqual(700f, scores[2].DistancePartie);
        Assert.AreEqual(600f, scores[3].DistancePartie);
        Assert.AreEqual(500f, scores[4].DistancePartie);
    }

    // Teste l'ajout de données invalides (distance ou durée négatives)
    [TestMethod]
    public void TestAjouterScore_DonneesInvalides()
    {
        bool ok = Course.AjouterScore(DB_TEST, "NegativeDistance", -100, 500, Course.TypesDeJeu.Distance, out string err);
        Assert.IsTrue(ok, "La base de données accepte les distances négatives");

        var scores = Course.LireScores(DB_TEST, 1, Course.TypesDeJeu.Distance, out _);
        Assert.AreEqual(-100f, scores[0].DistancePartie);

        ok = Course.AjouterScore(DB_TEST, "NegativeDuration", 100, -500, Course.TypesDeJeu.Chrono, out err);
        Assert.IsTrue(ok, "La base de données accepte les durées négatives");

        scores = Course.LireScores(DB_TEST, 1, Course.TypesDeJeu.Chrono, out _);
        Assert.AreEqual(-500f, scores[0].Duree);
    }

    // Teste la lecture de scores dans une base vide
    [TestMethod]
    public void TestLireScores_DatabaseVide()
    {
        var scores = Course.LireScores(DB_TEST, 5, Course.TypesDeJeu.Distance, out string err);

        Assert.IsNotNull(scores);
        Assert.AreEqual(0, scores.Count);
    }

    // Teste l'ajout de scores identiques pour plusieurs joueurs
    [TestMethod]
    public void TestAjouterScore_MultiplesJoueurs_MemeScore()
    {
        Course.AjouterScore(DB_TEST, "Joueur1", 500, 1000, Course.TypesDeJeu.Distance, out _);
        Course.AjouterScore(DB_TEST, "Joueur2", 500, 1000, Course.TypesDeJeu.Distance, out _);

        var scores = Course.LireScores(DB_TEST, 2, Course.TypesDeJeu.Distance, out _);

        Assert.AreEqual(2, scores.Count);
        Assert.AreEqual(500f, scores[0].DistancePartie);
        Assert.AreEqual(500f, scores[1].DistancePartie);
    }
}