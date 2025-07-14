using CourseVoiture;
namespace TestCourseVoiture;

/// <summary>
/// Tests de l'initialisation des parties (lecture des circuits, etc.)
/// </summary>
[TestClass]
[DoNotParallelize]
public class TestInitialisation
{
    // A remplir
// Teste la réinitialisation des variables distance, vitesse, état de la partie
[TestMethod]
    public void InitialiserPartie_ResetVariables()
    {
        var course = new Course();
        // Simule valeurs modifiées
        course.distanceTotale = 100;
        course.vitesseActuelle = 50;

        course.InitialiserPartie();

        Assert.IsTrue(course.PartieEnCours, "PartieEnCours doit être true après InitialiserPartie");
        Assert.AreEqual(Course.VITESSE_INITIALE, course.vitesseActuelle, "vitesseActuelle doit être remise à l'initiale");
        Assert.AreEqual(0, course.distanceTotale, "distanceTotale doit être remise à zéro");
    }

    // Teste que la méthode charge un parcours par défaut si le parcours est vide
    [TestMethod]
    public void InitialiserPartie_AvecParcoursVide_ChargeParDefaut()
    {
        var course = new Course();
        course.parcours = Array.Empty<string>();

        course.InitialiserPartie();

        Assert.IsTrue(course.parcours.Length > 0);
    }
}