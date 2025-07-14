using CourseVoiture;
namespace TestCourseVoiture;

/// <summary>
/// Tests des calculs de la vitesse (accélération)
/// </summary>
[TestClass]
[DoNotParallelize]
public class TestVitesse
{
    // A remplir
// Teste que la vitesse initiale augmente correctement après accélération
    [TestMethod]
    public void Accelerer_VitesseInitiale_AugmenteCorrectement()
    {
        var course = new Course();
        Assert.AreEqual(Course.VITESSE_INITIALE, course.vitesseActuelle);
        course.Accelerer();
        Assert.IsTrue(course.vitesseActuelle > Course.VITESSE_INITIALE, "La vitesse doit augmenter après Accelerer().");
    }

    // Teste que la vitesse ne dépasse pas la vitesse maximale après plusieurs accélérations
    [TestMethod]
    public void Accelerer_PlusieursFois_NeDepassePasMax()
    {
        var course = new Course();
        for (int i = 0; i < 100; i++) course.Accelerer();
        Assert.AreEqual(Course.VITESSE_MAX, course.vitesseActuelle, "La vitesse ne doit pas dépasser VITESSE_MAX.");
    }

    // Teste que la distance totale augmente après accélération
    [TestMethod]
    public void Accelerer_DistanceTotale_AugmenteAvecRoute()
    {
        var course = new Course();
        int before = course.distanceTotale;
        course.Accelerer();
        Assert.IsTrue(course.distanceTotale > before, "La distanceTotale doit augmenter après Accelerer().");
    }

    // Teste que la vitesse ne dépasse pas la limite maximale
    [TestMethod]
    public void Accelerer_VitesseMax_NeDepassePasLimite()
    {
        var course = new Course();
        course.vitesseActuelle = Course.VITESSE_MAX - 1;

        course.Accelerer();

        Assert.AreEqual(Course.VITESSE_MAX, course.vitesseActuelle);
    }
}