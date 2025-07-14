using CourseVoiture;
namespace TestCourseVoiture;

/// <summary>
/// Tests du constructeur du jeu et de la lecture du parcours
/// </summary>
[TestClass]
[DoNotParallelize]
public class TestConstructeur
{

    // Teste la lecture d’un fichier de parcours inexistant
    [TestMethod]
    public void LireParcours_FichierInexistant_RetourneFalse()
    {
        var course = new Course();
        bool result = course.LireParcours("fichier_inexistant.txt");
        Assert.IsFalse(result, "La lecture d'un fichier inexistant doit renvoyer false");
    }

    // Teste la lecture d’un fichier valide chargeant le parcours
    [TestMethod]
    public void LireParcours_FichierValide_RetourneTrueEtChargeParcours()
    {
        string tempFile = Path.GetTempFileName();
        try
        {
            // Crée un fichier avec HauteurJeu lignes de LargeurJeu 'X'
            using (var sw = new StreamWriter(tempFile))
            {
                for (int i = 0; i < Course.HauteurJeu; i++)
                    sw.WriteLine(new string('X', Course.LargeurJeu));
            }
            var course = new Course();
            bool result = course.LireParcours(tempFile);
            Assert.IsTrue(result, "La lecture d'un fichier valide doit renvoyer true");
            Assert.IsNotNull(course.parcours);
            Assert.AreEqual(Course.HauteurJeu, course.parcours.Length);
            Assert.IsTrue(course.parcours[0].StartsWith("XXX"));
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    // Vérifie que le constructeur initialise bien un tableau de parcours par défaut
    [TestMethod]
    public void Constructeur_ParcoursParDefaut_InitialiseTableau()
    {
        var course = new Course();
        Assert.IsNotNull(course.parcours, "Le parcours ne doit pas être null après construction");
        Assert.AreEqual(Course.HauteurJeu, course.parcours.Length, "La hauteur du parcours doit correspondre à HauteurJeu");
        foreach (var ligne in course.parcours)
        {
            Assert.AreEqual(Course.LargeurJeu, ligne.Length, "Chaque ligne du parcours doit avoir LargeurJeu caractères");
        }
    }

    // Teste que charger un parcours au hasard dans un dossier vide ou inexistant retourne false
    [TestMethod]
    public void ChargerParcoursAuHasard_DossierVide_RetourneFalse()
    {
        var course = new Course();
        bool result = course.ChargerParcoursAuHasard("folder_dont_exist");
        Assert.IsFalse(result, "ChargerParcoursAuHasard doit renvoyer false si le dossier est vide ou inexistant");
    }

    // Teste que charger un parcours au hasard dans un dossier avec un fichier retourne true
    [TestMethod]
    public void ChargerParcoursAuHasard_DossierAvecFichier_RetourneTrue()
    {
        string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        try
        {
            string file = Path.Combine(tempDir, "circuit.txt");
            // Création d'un parcours simple
            char[] routeParcours = { 'X', 'Y', 'Z' }; // Exemple de caractères pour le parcours
            File.WriteAllLines(file, Enumerable.Repeat(new string(routeParcours[0], Course.LargeurJeu), Course.HauteurJeu));
            var course = new Course();
            bool result = course.ChargerParcoursAuHasard(tempDir);
            Assert.IsTrue(result, "ChargerParcoursAuHasard doit renvoyer true si un fichier de parcours existe");
            Assert.IsNotNull(course.parcours, "Le parcours doit être chargé après réussite");
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }

    // Teste le chargement au hasard dans un dossier contenant plusieurs fichiers
    [TestMethod]
    public void ChargerParcoursAuHasard_DossierAvecPlusieursFichiers_RetourneTrue()
    {
        string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        try
        {
            // Création de plusieurs fichiers de parcours
            char[] RouteParcours = { 'X', 'Y', 'Z' }; // Exemple de caractères pour le parcours
            string file1 = Path.Combine(tempDir, "circuit1.txt");
            string file2 = Path.Combine(tempDir, "circuit2.txt");
            string file3 = Path.Combine(tempDir, "circuit3.txt");

            // Exemple de parcours simple
            File.WriteAllLines(file1, Enumerable.Repeat(new string(RouteParcours[0], Course.LargeurJeu), Course.HauteurJeu));
            File.WriteAllLines(file2, Enumerable.Repeat(new string(RouteParcours[0], Course.LargeurJeu), Course.HauteurJeu));
            File.WriteAllLines(file3, Enumerable.Repeat(new string(RouteParcours[0], Course.LargeurJeu), Course.HauteurJeu));

            var course = new Course();
            bool result = course.ChargerParcoursAuHasard(tempDir);

            Assert.IsTrue(result, "ChargerParcoursAuHasard doit renvoyer true si des fichiers de parcours existent.");
            Assert.IsNotNull(course.parcours, "Le parcours doit être chargé après réussite.");
            Assert.IsTrue(course.parcours.Length > 0, "Le parcours chargé ne doit pas être vide.");

        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }

    // Vérifie que les murs sont bien placés aux bords du parcours par défaut
    [TestMethod]
    public void Constructeur_ParcoursParDefaut_AvecMursAuxBords()
    {
        var course = new Course();
        int gauche = (Course.LargeurJeu - Course.LargeurRoute) / 2;
        int droit = gauche + Course.LargeurRoute - 1;
        foreach (var ligne in course.parcours)
        {
            // bords de route en mur
            Assert.AreEqual(Course.MurParcours[0], ligne[gauche], "Mur gauche absent");
            Assert.AreEqual(Course.MurParcours[0], ligne[droit], "Mur droit absent");

            for (int j = 0; j < Course.LargeurJeu; j++)
            {
                if (j > gauche && j < droit)
                    Assert.AreEqual(Course.RouteParcours[0], ligne[j], "Route absente à l'intérieur");
                else if (j < gauche || j > droit)
                    Assert.AreEqual(Course.HerbeParcours[0], ligne[j], "Herbe absente à l'extérieur");
            }
        }
    }

    // Teste la lecture d’un fichier trop court
    [TestMethod]
    public void LireParcours_FichierTropCourt_RetourneFalse()
    {
        var course = new Course();
        string tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, "ligne1\nligne2");
        bool result = course.LireParcours(tempFile);
        Assert.IsFalse(result);
    }

    // Teste que le constructeur utilise le parcours par défaut si fichier invalide
    [TestMethod]
    public void Constructeur_AvecFichierInvalide_UtiliseParcoursParDefaut()
    {
        var course = new Course("fichier_inexistant.txt");
        Assert.IsNotNull(course.parcours);
        Assert.AreEqual(Course.HauteurJeu, course.parcours.Length);
    }
}