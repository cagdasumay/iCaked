using Cake.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Caching;
using System.ServiceModel.Syndication;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Xml;

namespace Cake.Controllers
{
    public class UtilityController : Controller
    {
        private DatabaseController dc = new DatabaseController();

        // This controller contains generally contains utility methods to be used throughout the site

        public String[] bakeryMarketFooter(String cat)
        {
            String title = ""; String description = "";

            if (cat == "Yaş Pasta")
            {
                title = "Delicious Delicious and Fresh Pastries at iCaked.com.";
                description = @"Davetlerde, düğünlerde, yıldönümlerinde, doğum günlerinde, kısaca özel günlerde yaş pasta kesilir. Bu gelenek asırlar öncesine dayanır, insanların en keyifli günlerinde yaş pastalara sarılmasının sebebi ise tatlarının ve görünüşlerinin insanları mutlu etmesidir. 13.YY’da İngilizlerin “CAKE” sözcüğünü kullanmaya başlamasıyla oluşan bu serüvenin sorumluluğunu bugün, dünya üzerinde eşi benzeri olmayan ve pastacılık sektöründe çığır açan iCaked.com üstlenmiştir. <br /><br />           
                Pastalar ile güncellik iç içedir ve onlara mutluluk, lezzet ve estetikliği koyan, son dokunuş olanağını sizlere sunan iCaked.com’dur. <br /><br />
                Yaş pastalar üzerlerine sayılar, kelimeler, şekiller ve mumlar koyularak süslenir. 1920 yılında “Happy Birthday To You” şarkısı meşhur olduktan sonra dünya genelinde pastaların üzerlerine “Happy Birthday” yazılmaya başlanmıştır. Yaş pastalar bu sayede güncellikle beraberdir.<br /><br />
                Karışık meyveli, çikolatalı, çilekli, frambuazlı, fıstıklı, fıstık krokanlı, kestaneli, vişneli ve beyaz çikolatalı olan yaş pastalar en çok tercih edilen yaş pasta türleridir, dolayısıyla üretimi en çok olan yaş pasta türleri de bunlardır.Dış görünüşleriyle insanı cezbeden yaş pastalar, içlerinde de bir hazine taşır. Ağız da kremanın verdiği yumuşaklıkla birlikte eriyormuş hissi yaratması da cabası.<br /><br />
                Bir arkadaşınızın, çok sevdiğiniz bir insanın ya da bir akrabanızın doğum günü.Bir doğum günü partisi var ve orada bulunması gereken ve herkesin aklına gelen ilk şey doğum günü olan kişinin kesmesi, hatta üzerinde ki mumları dileğini tuttuktan sonra üfleyerek söndürülmesi beklenen bir yaş pastadır. Kısacası, yaş pastalar doğum günlerinin vazgeçilmez bir parçasıdır.<br /><br />
                Yaş pasta seçiminizi en yakın pastaneye gidip pastane vitrininden yapmak yerine neden iCaked.com üzerinden yapmanızın daha mantıklı bir seçim olduğunu tek bir cümleyle açıklayabiliriz. Sadece kaliteli pastanelerle çalışıyoruz. Bunu, sadece sizleri memnun etmek, daha iyi bir pasta lezzeti ve görünümü sunmak ve daha güzel ve lezzetli doğum günleri, yıl dönümleri, düğünler ve özel günler geçirmenizi sağlamak için yapıyoruz. <br /><br />
                Evinizde, iş yerinizde, yolda ya da dışarıda, dilediğiniz zamanda ve dilediğiniz lezzete sahip bir yaş pastayı iCaked.com’dan online sipariş verebilirsiniz. <br /><br />
                Tüm pastanelerin yaş pasta ürünlerine ulaşmak için <a href='/tezgah-pastalari'>tıklayın</a>
                ";
            }
            else if (cat == "1 Yaş Pastaları")
            {
                title = "En Güzel ve En Şirin 1 Yaş Butik Pastalar iCaked.com’da.";
                description = @"Tasarım butik pasta denilince akla ilk gelen iCaked.com, şimdi ise yeni doğmuş bireyler, canlarımız olan 1 yaşına girmiş ya da 1 yaşına girmek üzere olan bebekler için mükemmel bir kutlama hediyesi sunuyor. Bir çiftin Dünya’ya verdiği bu temiz ve güzel kokulu hediye için, ufak ve anlamlı bir 1 yaş butik pasta hediyesi. <br /><br />
                Çevrenizde 1 yaşını doldurmak üzere tertemiz kokulu, şirin bir bebek var ve bir hoş geldin partisi verilecek. Eğer partiye katılacaksanız, unutmamanız gereken 1 yaş tasarım butik pastalarımızdan bir tanesini online sipariş vermek. Bebekler için hediye seçimi zor bir seçimdir ve bu zorluktan sizleri kurtarıyoruz. Hem hoş geldin partisine katılacakların hem de ebeveynlerinin ağızlarını tatlandıracak enfes bir 1 yaş tasarım butik pasta ile gitmenizi tavsiye ediyoruz.<br /><br />
                Hayatınızda yeni ve yoğun bir döneme girdiniz, yani ebeveyn oldunuz. Bebeğiniz ile mutlu ve yorucu bir hayat sürüyorsunuz ve o ilk yaşını bitirecek. 1. Yaşını kutlamak için ayarladığınız partide, yemekten sonra lezzetli bir butik pasta hiçte fena olmaz. Eğer üç kişilik bir kutlama olacaksa, iCaked.com’dan vereceğiniz online sipariş üzerine gelecek bir 1 yaş tasarım butik pasta ile partiyi lezzetlendirebilirsiniz. Kutlamaların vazgeçilmezleri eşsiz lezzetleriyle ve iCaked.com farkıyla butik pastalardır.<br /><br />
                Doğumdan sonra ki yoğun temponuzda bebeğinize 1 yaş partisi düzenlemek istiyorsanız ya da eşinizle baş başa bir kutlama yapacaksanız, iCaked.com günü lezzetli geçirmeniz için elbette tek adres. Online vereceğiniz bir 1 yaş tasarım butik pasta siparişi ile günü ve o gün yanınızda olacak kişilerin gününü eşsiz ve lezzetli bir güne çevirebilirsiniz.  <br /><br />
                iCaked.com’dan 1 yaş tasarım butik pasta siparişini herkes verebilir, çünkü her bütçeye uygun pastalar sunuyoruz. Kendi bütçenize göre bir 1 yaş tasarım butik pasta beğenebilir ya da 1 yaş pastanızı kendiniz tasarlayabilirsiniz. Tasarlama seçeneğine göz atmak isterseniz <a target='_blank' href='https://www.icaked.com/Editor'>linke</a> tıklamanız sizi, hayal gücünüzle baş başa bırakır. Tasarım butik pastalarımızın fiyatlandırmasında pasta boyutu (kaç kişilik olacağı) ve yapım zorluğu etkenlerini göz önünde bulunduruyoruz. Bu sayede kendinizi zorlamadan enfes bir tasarım butik pastayı online sipariş vererek şahane bir özel gün geçirebilir, sevdiklerinizi mutlu edebilirsiniz.
                <br /><br />        
                Tüm pastanelerin 1 Yaş Pastalarına ulaşmak için <a href='/butik-pastalar/1-yas-pastalari'>tıklayın</a>
                ";
            }
            else if(cat == "Bebek Pastaları")
            {
                title = "En Sevimli ve Sevgi Dolu Bebek Butik Pastalar iCaked.com’da.";
                description = @"Tasarım butik pasta denilince akla ilk gelen iCaked.com, şimdi ise yeni doğmuş bireyler, canlarımız olan bebekler için mükemmel bir kutlama hediyesi sunuyor. Bir çiftin Dünya’ya verdiği bu temiz ve güzel kokulu hediye için, ufak ve anlamlı bir bebek butik pasta hediyesi. <br /><br />
                Çevrenizde yeni bir bebek doğmak üzere ve bir hoş geldin partisi verilecek ve partiye katılacaksanız ya da henüz yeni doğmuş ya da 1 yaşını doldurmamış tertemiz kokulu ve şirin bir bebek var ve onu sevmeye gidecekseniz bebek tasarım butik pastalarımızdan bir tanesini online sipariş vermeyi unutmayın. Zor olan bebekler için hediye seçmek işinden sizleri kurtarıyoruz ve hem hoş geldin partisine katılacakların hem de ebeveynlerinin ağızlarını tatlandıracak enfes bir pasta ile gitmenizi şiddetle tavsiye ediyoruz.<br /><br />
                Hayatınızda duyduğunuz en iyi haberi aldınız. Doktora gittiniz ve yapılan testler sonucu hamile olduğunuzu öğrendiniz ya da karınız bir gün sizi aradı ve bir bebek sahibi olacağınızı söyledi. Kutlamak için ayarladığınız yemekten sonra lezzetli bir butik pasta hiçte fena olmaz, üç kişilik fotoğraf çekme deneyimlerine iCaked.com’dan vereceğiniz online sipariş üzerine gelecek bir bebek butik pasta ile hazırlık yapabilirsiniz. Şüphesiz ki, kutlamaların vazgeçilmezleri eşsiz lezzetleriyle butik pastalardır.<br /><br />
                Yeni doğum yaptıysanız ya da eşiniz yeni doğum yaptıysa, kısacası bir bebek sahibi olduysanız ve bebeğinize hoş geldin partisi düzenlemek istiyorsanız ya da eşinizle baş başa bir kutlama yapacaksanız, günü lezzetli geçirmeniz için tek adres elbette iCaked.com. Online vereceğiniz bir bebek tasarım butik pasta sipariş ile günü ve o gün yanınızda olacak kişilere eşsiz ve lezzetli bir güne çevirebilirsiniz.  <br /><br />
                iCaked.com’dan bebek tasarım butik pasta siparişini herkes verebilir, çünkü her bütçeye uygun pastalar sunuyoruz. Kendi bütçenize göre bir bebek tasarım butik pasta beğenebilir ya da bebek butik pastanızı kendiniz tasarlayabilirsiniz. Tasarlama seçeneğine göz atmak isterseniz <a target='_blank' href='https://www.icaked.com/Editor'>linke</a> tıklamanız hayal gücünüzle baş başa kalmak için yeterli olacaktır. Tasarım butik pastalarımızın fiyatlandırmasında pasta boyutu (kaç kişilik olacağı) ve yapım zorluğu etkenlerini göz önünde bulunduruyoruz. Bu sayede kendinizi zorlamadan enfes bir tasarım butik pastayı online sipariş vererek şahane bir özel gün geçirebilir, sevdiklerinizi mutlu edebilirsiniz.
                <br /><br />        
                Tüm pastanelerin Bebek Pastalarına ulaşmak için <a href='/butik-pastalar/bebek-pastalari'>tıklayın</a>                
                ";
            }
            else if (cat == "Butik Pasta")
            {
                title = "Büyüleyici, Harika ve Taze Butik Pastalar için iCaked.com";
                description = @"Davetler, yıldönümleri, düğünler, doğum günleri, kısaca hiçbir özel gün yaş pastasız geçmez. Aslında geçmezdi. Dünya üzerinde eşi benzeri olmayan ve pastacılık sektöründe çığır açan iCaked.com yaş pastaların yerine butik pastaları getirdi. Birbirinden şık tasarımları, eşsiz lezzetleriyle tasarım butik pastalar, artık özel günlerin favori pasta çeşidi oldu. Butik tasarım pastalar ile güncellik iç içedir ve onlara mutluluk, lezzet ve estetikliği koyan, son dokunuş olanağını sizlere sunan iCaked.com’dur.<br /><br />
                Karışık meyveli, çikolatalı, çilekli, frambuazlı, fıstıklı, fıstık krokanlı, kestaneli, vişneli, beyaz çikolatalı ve daha fazla içerik seçeneği olan butik pastalar en çok tercih edilen butik pasta içerikleridir. Dış görünüşleriyle insanı kendine hayran eden butik pastalar, içlerinde de bir hazine taşırlar. Kesmeye kıyamayacağınız bu tasarım butik pastalar, kolay sipariş ve memnuniyet seviyesinin çok yüksek olmasından dolayı, Ankara ve İstanbul başta olmak üzere, birçok şehrimizdeki davetler, yıldönümleri, düğünler, doğum günlerinin vazgeçilmez parçaları oldular.<br /><br />
                Bir arkadaşınızın, çok sevdiğiniz bir insanın ya da bir akrabanızın doğum günü yaklaşıyorsa ve bir doğum günü partisi olacaksa, orada bulunması gereken ve herkesin aklına gelen ilk şey doğum günü olan kişinin kesmesi, hatta üzerinde ki mumları dileğini tuttuktan sonra üfleyerek söndürülmesi beklenen bir pastadır. Kısacası, tasarım butik pastalar İstanbul ve Ankara’daki doğum günü partilerinin vazgeçilmez bir parçasıdır.<br /><br />
                Butik pasta üretimini her pastane gerçekleştirmiyor. İstanbul ve Ankara gibi büyük şehirlerde yaşamıyorsanız, iCaked.com’da yer alan yüzlerce enfes tasarım butik pasta gibi bir tasarım butik pasta bulmanız çok kolay olmaz. Eğer Ankara ya da İstanbul’da yaşıyorsanız, seçiminizi en yakın pastaneye gidip pastane vitrininden yapmak yerine iCaked.com üzerinden yapmanızın daha mantıklıdır, çünkü sadece kaliteli pastanelerle çalışıyoruz. Bunu, sadece sizleri memnun etmek, daha iyi bir pasta lezzeti ve görünümü sunmak ve daha güzel ve lezzetli doğum günleri, yıl dönümleri, düğünler ve özel günler geçirmenizi sağlamak için yapıyoruz. <br /><br />
                Evinizde, iş yerinizde, yolda ya da dışarıda, dilediğiniz zamanda, dilediğiniz lezzete sahip bir tasarım butik pastayı iCaked.com’dan online sipariş verebilirsiniz. 
                <br /><br />        
                Tüm pastanelerin Butik Pastalarına ulaşmak için <a href='/butik-pastalar'>tıklayın</a>                 
                ";
            }
            else if (cat == "Çikolata")
            {
                title = "iCaked.com üzerinden online çikolata siparişi verebilirsiniz.";
                description = @"Çikolata.<br /><br />
                Mutlu olmak istenildiğinde, stres altındayken, kaçamak yapmak için, ağız tatlandırmak için kısacası zor ya da mutlu her zaman yanımızda olan yiyecektir çikolata. Küçük bir haz için ağıza bir parça çikolata alınır ve eriyene kadar beklenir, kafadaki bütün sorunlar silinir o süre için, suratta ise bir gülümseme…<br /><br />
                Bayramların da vazgeçilmezidir çikolata, her eve kilolarca alınır ki misafir geldiğinde ikram edilebilsin, misafirlerin ağzı tatlansın. Mutluluğun adeti diyebiliriz çikolata için. Aynı zamanda tatlılar arasında en hafif olandır ve sevmeyeni az bulunur.<br /><br />
                Dünya çapında geniş bir kitle tarafından sevilerek tüketilen bir ürün olan çikolata, genelde keyif için tüketilen bir gıda olarak bilinmesinin yanı sıra, son yıllarda birçok insan için dengelenmiş diyetlerine sağlığın bir parçası olarak dahil edilmektedir. Yapılan araştırmalar çikolatanın kalp rahatsızlıklarını ve bazı kanser çeşitlerini azalttığı yönündedir. Buna ek olarak; Yüzyıllar boyunca zayıf düşmüş hastaların kilo almaları için tedavi edilmelerinde, umursamaz, bitkin veya güçsüz hastaların sinir sistemlerini uyarmada ve sindirimi arttırmada, mide rahatsızlıklarında, böbrekleri uyarmada ve bağırsak fonksiyonunu arttırmada kullanılmıştır. Tedavi aşamasında yardımı fazlasıyla gözüktüğü diğer hastalıklar ise; anemi, iştahsızlık, zihin yorgunluğu, memede yetersiz süt üretimi, tüberküloz, ateş, gut, böbrek taşları, kısa ömür olarak belirtilmiştir.<br /><br />
                Çikolatanın tadı en önemli özelliğinden biridir. Kakao yağı vücut ısısına yakın bir erime noktasına sahiptir. Bu yüzden çikolata yendiğinde ağızda akıcı, ağzı kaplayan katıdan sıvıya dönen bir yapıdadır. Çikolatalar gerek içermiş oldukları besleyici öğeler açısından ve gerekse sahip oldukları lezzet, çeşni ve aromaları açısından her zaman insanlar tarafından istekle tüketilen bir besin maddesi olmuştur.<br /><br />
                Bir insanı tebrik etmek için, teşekkür sunmak için ya da özür dilemek için çikolata hediye etmek güzel bir tercihtir. İnsanlara çikolata seti ya da çikolata sepeti hediye etmek onları mutlu eder. iCaked.com ile sevdiklerinize ya da mutlu etmek istediklerinize çikolata hediye etmek çok kolay! iCaked.com üzerinden online sipariş vererek insanlara tebriğinizi, teşekkürünüzü ya da özrünüzü çikolata hediye ederek gösterebilirsiniz. 
                ";
            }
            else if (cat == "Çizgi Film Pastaları")
            {
                title = "Çizgi Film Pastaları Dendiğinde Akla İlk Gelen iCaked.com";
                description = @"Çizgi filmler çocukların vazgeçilmezidir. Çocukların, hayal güçleri çizgi filmler sayesinde gelişir, onlar yönlendirir. Çizgi film izleyen neredeyse her çocuğun bir çizgi kahramanı vardır. Kahramanları gibi olmak isterler, onlar gibi giyinmeye, davranmaya çalışırlar. Bu konu çok önemlidir çünkü insanlar hayallerinden peşinden koştukça gelişir. Merak etmeyin, yardımınıza iCaked.com yetişiyor!<br /><br />
                Rengarenk tasarımlarıyla ve enfes görünümleriyle Çizgi Film Butik Pastalar, hem çocukların ufkunu açmaya yardım edecek, hem de onların özel günlerini lezzetlendirecek. Her şeyi hak eden, geleceğin umudu çocuklarımızın doğum, kutlama ve parti günleri için, iCaked.com olarak elimizden geleni yapıyoruz, onların mutluluğuna önem veriyoruz. Bir doğum günü partisinde ya da kutlama gibi özel günlerden birinde, iCaked.com üzerinden online sipariş verilmiş bir pasta ya da iCaked.com editöründe tasarlanmış bir pastanın masanın üzerinde durması hem çocuklar için hem de yetişkinler için mutluluk ve neşe kaynağıdır. Çünkü biliyorsunuz ki, sizler ve çocuklar için en kaliteli ve en lezzetli pastaları sizlere sunuyoruz.<br /><br />
                Pastaların içeriklerine karar vermek her zaman kolay bir durum değildir. Herkesin damak zevki farklı olmasına rağmen çocukların damak zevkleri birdir. Pasta! Tatlı ve renkli yiyecekler tüm çocukların ilgisini çeker. Doğum günlerinde, özel günlerde veya bir olayı kutlarken birbirinden renkli, birbirinden lezzetli yüzlerce çizgi film pastası arasından dilediğiniz bir butik pastayı iCaked.com farkıyla online sipariş verebilirsiniz.<br /><br />
                iCaked.com’dan çizgi film pasta siparişi vermeniz için bir başka gerekçe ise her bütçeye uygun pastalar sunmamız. Kendi bütçenize göre bir tasarım butik pasta beğenebilir ya da butik pastanızı kendiniz tasarlayabilirsiniz. Tasarlama seçeneğine göz atmak isterseniz <a target='_blank' href='https://www.icaked.com/Editor'>linke</a> tıklamanız hayal gücünüzle baş başa kalmak için yeterli olacaktır. Tasarım butik pastalarımızın fiyatlandırmasında pasta boyutu (kaç kişilik olacağı) ve yapım zorluğu etkenlerini göz önünde bulunduruyoruz. Bu sayede kendinizi de zorlamadan enfes bir tasarım butik pastayı online sipariş vererek şahane bir özel gün geçirebilir, sevdiklerinizi mutlu edebilirsiniz.<br /><br />
                Tamamen size özel olan bir tasarım pasta için siz tasarlayın biz pastalayalım!
                <br /><br />        
                Tüm pastanelerin Çizgi Film Pastalarına ulaşmak için <a href='/butik-pastalar/cizgi-film-pastalari'>tıklayın</a> 
                ";
            }
            else if (cat == "Doğum Günü Pastaları")
            {
                title = "En Kaliteli ve En Lezzetli Doğum Günü Pastaları iCaked.com’da.";
                description = @"Doğum günleri ve doğum günü pastaları ayrılmaz ikililerdir. Doğum günü denildiğinde akla iki şey gelir, doğum günü hediyeleri ve doğum günü pastası.<br /><br />
                Doğum günlerinde olmazsa olmaz pasta kesme adeti 13.YY’dan, Almanların çocuklara gösterdiği fazla ilgiden doğmuştur. O dönemlerde, çocukların bu özel günlerinde, pastanın üzerine doğum günü sahibi çocuğun yaşından bir fazla sayıda mum konuluyordu. O bir fazla mum, bir gün sönecek hayatın ışığını simgeliyordu.<br /><br />
                Güzel bir doğum günü partisi geçirmeniz adına, iCaked.com olarak, pasta sektöründe markalaşmış, en iyi hizmeti sunan ve en taze pastaları evinize kadar getiren pastanelerle anlaşıyoruz. Birden çok pastaneyle anlaşmamızın sizlere katkısı ise çok geniş bir pasta yelpazesi yaratması. Daha güzel doğum günleri için, daha önce tatmadığınız pasta lezzetleri ve sevdiğiniz pastaların en kalitelilerini sizlere sunuyoruz.<br /><br />
                Butik pastalar, sınırlarının hayal gücü kadar olması ve yapım aşaması tamamlanmış olan pastayı gören herkesin hayranlıkla ona bakması sayesinde oldukça yaygınlaştı. ‘Doğum Günlerinde Neden butik pasta?’ sorusuna özetle cevap vermek gerekirse, doğum günlerinde insanlar sıradanlaşan bileklik, saat ya da kitap almaktan sıkılmış durumdalar. Burada imdadımıza butik pastalar yetişiyor. Çünkü, artık iCaked.com sayesinde istediğimiz tasarımı <a target='_blank' href='https://www.icaked.com/Editor'>yapabiliyoruz</a> ya da hazır tasarımları kullanarak doğum günü sahibini sembolize edebiliyoruz, bu sayede yeni yaşına girecek olan kişiyi mutlu edebiliyoruz. Tasarımın altında yatan efsane lezzetler ise cabası, bu sayede de pastanın kesileceği yere yakın olan herkesi mutlu ediyoruz. ‘Doğum Günlerinde Neden butik pasta’ sorusuna daha detaylı bir cevabımızı ise <a target='_blank' href='http://blog.icaked.com/2017/01/21/neden-butik-pasta/'>bloğumuzda bulabilirsiniz.</a>
                <br /><br />        
                Tüm pastanelerin Doğum Günü Pastalarına ulaşmak için <a href='/butik-pastalar/dogum-gunu-pastalari'>tıklayın</a>                 
                ";
            }
            else if (cat == "Kutlama Pastaları")
            {
                title = "En Taze ve En Kaliteli Kutlama Pastaları iCaked.com’dan sipariş verilir.";
                description = @"Karışık meyve, çikolata, çilek, frambuaz, fıstık, fıstık krokan, kestane, vişne, beyaz çikolata, badem, limon, böğürtlen seçeneklerini ve daha fazlasını pasta içeriğiniz olarak belirleyebileceğiniz, online sipariş sitesindesiniz. iCaked.com olarak, en çok tercih edilen butik ve tasarım pasta türlerini, sizler daha lezzetli ve daha keyifli bir gün geçirmeniz için sizlere sunuyoruz. Dış görünüşleriyle insanı cezbeden tasarım butik pastalar, içlerinde de lezzet abidesi barındırır. <br /><br />
                Bir arkadaşınızın, çok sevdiğiniz bir insanın ya da bir akrabanızın doğum günü. Bir doğum günü partisi var ve orada bulunması gereken ve herkesin aklına gelen ilk şey doğum günü olan kişinin kesmesi, hatta üzerinde ki mumları, dileğini tuttuktan sonra üfleyerek söndürülmesi beklenen bir pastadır. Kısacası, pastalar doğum günlerinin vazgeçilmez bir parçasıdır. Sadece doğum günleri de değil, her türlü kutlama temalı günlerinize eşlik edecek olan şey, iCaked.com üzerinden sipariş verilen bir tasarım butik pastadır. Terfi alan bir yakınınızın, doğum yapan bir yakınınızın ya da sizin için anlam taşıyan mutlu bir gününüze yine eşlik edecek olan, tasarımıyla gözlerinizi doyuracak, lezzetiyle de keyfinizi yerine getirecek bir tasarım butik pasta.<br /><br />
                Butik pasta seçiminizi yakınınızdaki bir pastaneye gidip oranın vitrininden yapmak yerine neden iCaked.com üzerinden yapmanızın daha mantıklı bir seçim olduğunu şöyle açıklayabiliriz. Sadece kaliteli pastanelerle çalışıyoruz. Bunu, sadece sizleri memnun etmek, daha iyi bir pasta lezzeti ve görünümü sunmak ve daha güzel ve lezzetli doğum günleri, yıl dönümleri, düğünler ve özel günler geçirmenizi sağlamak için yapıyoruz. Bunun yanı sıra, pastaneye gittiğinizde, sadece o an vitrinde bulunan pastalardan birini alabilirsiniz. iCaked.com bunu da ortadan kaldırıyor! Bu işi online halletmenin artısı, sitemizde o an pastanenin üretebileceği bütün pastaları görebilmeniz ve o pastayı sizin için yaptıktan sonra, istediğiniz adrese getirmeleri. Kısaca, evinizde, iş yerinizde, yolda ya da dışarıda, dilediğiniz zamanda ve dilediğiniz lezzete sahip bir butik pastayı iCaked.com’dan online sipariş verebilirsiniz. <br /><br />
                Eğer pastanelerimizin ya da kullanıcılarımızın tasarladığı butik pastalardan birini beğenemediyseniz ve pastanızın tamamen size özel olmasını istiyorsanız, sizi hayal gücünüzün bu pastayı yaratabileceği editörümüze bekliyoruz! <a target='_blank' href='https://www.icaked.com/Editor'>Siz tasarlayın biz pastalayalım!</a>
                <br /><br />        
                Tüm pastanelerin Kutlama Pastalarına ulaşmak için <a href='/butik-pastalar/kutlama-pastalari'>tıklayın</a>                 
                ";
            }
            else if (cat == "Sevgili Pastaları")
            {
                title = "Sevgi ve Lezzet Dolu Sevgili Butik Pastaları İçin iCaked.com";
                description = @"Sevgili sizi sadece kim olduğunuz için değil, siz olduğunuz için seven, size sadece bakışlarıyla sevgisini ve sizi kendinizi önemli hissettirebilen kişidir. Sevgi ve bağlılık duyulan bu kişilerin mutluluğu çok önemlidir. Genellikle hayatımızda en önemli ilk iki şey aile ve sevgili olur. Hayatı paylaşmanın farkına vardıran, o insandan ayrılınca özlemeye başlanan kişi, bazen hayattan her şeyden önemli bir hâl alabilir. Kendi mutluluğunuz yerine, sevgilinizin mutluluğunu koyarsınız, sadece, sevgiliniz mutlu olsun istersiniz. O gülümsedikçe siz de gülümsersiniz.<br /><br />
                Sevgiliyi gülümsetmek konusunda iCaked.com olarak, fazlasıyla iddialıyız. Bu iddiamız, pastacılık sektöründe çığır açmamıza ve butik pastadaki kalitemize dayanıyor. Pastacılık sektörüne online pasta tasarlama özelliğini getirerek, tasarım butik pasta sektörüne yeni bir perde açan iCaked.com olarak, sevgili butik pasta kategorisinde, anlaştığımız en kaliteli pastaneler, kaliteli hizmet ve en lezzetli pastalarla karşınızdayız.<br /><br />
                İstanbul ya da Ankara’daki sevgilinizin içini ısıtacak, gördüğünde kendine hayran edecek ve gününü lezzetlendirecek, bir daha asla unutamayacağı sevgili tasarım butik pasta hediye etmeniz için online sipariş vermeniz yeterlidir. Sevgili butik pasta kategorisinde ki onlarca butik pasta arasından seçeceğiniz bir tasarım pastayı direkt olarak ya da üzerinde oynamalar yaparak online sipariş verebilirsiniz. Sevgili tasarım butik pastanızın içeriğini dilediğiniz gibi seçebilir, butik pastanızın üzerine pastaneye iletilmesini istediğiniz bir not bırakabilirsiniz. Sevgili butik pastanızın üzerine resim koymak isterseniz de dosyalarınız içerisinden istediğiniz bir fotoğraf seçmeniz yeterli olacaktır.<br /><br />
                iCaked.com sevgili butik pasta tasarımları arasından bir hazır tasarım beğenemediyseniz üzülmeyin. Sizin için tasarladığımız editörümüzü denemenizi tavsiye ederiz. Dilediğiniz bir şekilde, dilediğiniz renkte, istediğiniz iki boyutlu objelerle ya da üç boyutlu objelerle, yan süslemelerle ya da kenar süsleriyle, yazılarla, hatta bir resimle hayal gücünüzün sınırlarını zorlayarak ya da gerçekliği yansıtarak, kendi butik pastanızı tasarlayabilirsiniz. <a target='_blank' href='https://www.icaked.com/Editor'>Editöre bu linkten ulaşabilirsiniz</a>.  <br /><br /> 
                Uzak bir ilişkiniz varsa, yani siz bir şehirde sevgiliniz ise Ankara’da ya da İstanbul’da yaşıyorsa, sevgilinize bir sevgili butik pasta hediye ederek sevginizi gösterebilirsiniz. <br /><br />
                Sevgili ve butik pasta arasındaki bağlantının daha detaylı incelemesini iCaked.com’un hazırladığı <a target='_blank' href='http://blog.icaked.com/2017/03/14/sevgiliye-butik-pasta/'>blog’da bulabilirsiniz</a>
                <br /><br />        
                Tüm pastanelerin Sevgili Pastalarına ulaşmak için <a href='/butik-pastalar/sevgili-pastalari'>tıklayın</a>                 
                ";
            }
            return new String[] { title, description };
        }

        public String productCatCorrector(DataRow product)
        {
            String result = "";
            if (!product.Table.Columns.Contains("Select"))
            {
                if (String.IsNullOrEmpty(product["SizeOptions"].ToString()) == false) { result = "For " + product["SizeOptions"].ToString().Split(' ')[0] + " people"; }
                else if (String.IsNullOrEmpty(product["Gram"].ToString()) == false) { result = product["Gram"].ToString().Split(' ')[0] + " gram"; }
                else if (String.IsNullOrEmpty(product["Number"].ToString()) == false) { result = product["Number"].ToString().Split(' ')[0] + " many"; }
            }
            else
            {
                if (String.IsNullOrEmpty(product["SizeOptions"].ToString()) == false) { result = "For " + product["Select"].ToString().Split(' ')[0] + " people"; }
                else if (String.IsNullOrEmpty(product["Gram"].ToString()) == false) { result = product["Select"].ToString().Split(' ')[0] + " gram"; }
                else if (String.IsNullOrEmpty(product["Number"].ToString()) == false) { result = product["Select"].ToString().Split(' ')[0] + " many"; }
            }
            return result;
        }  

        public List<DataTable> GetIndexCakes()
        {
            String[] ba = new String[] { "8 Mart Kadınlar Günü Butik Pasta", "8 Mart Kadınlar Günü Çiçekli Butik Pasta", "Kalp Balonlu Sevgililer Günü Butik Pasta", "Karlar Ülkesi Özel Pasta" };
            String[] ya = new String[] { "New York Pasta", "Liva Gizemi", "Şeftalili Pasta", "Çilekli Kalpli Özel Yaş Pasta" };
            String[] bi = new String[] { "Özel Tasarım Anne Butik Pasta", "İlk Diş Yaş Pasta", "Winnie the Pooh Tasarım Pasta", "Yeni Evli Şirin Butik Pasta" };
            String[] yi = new String[] { "Frambuazlı ve Çikolatalı Yaş Pasta", "Böğürtlen ve Çikolatalı Yaş Pasta", "Frambuazlı Yaş Pasta", "Karışık Meyveli Yaş Pasta" };
            String[] tt = new String[] { "Mylittle Pony Özel Pasta", "Noel Baba Aranalı Yılbası Pastası", "Jiggly Puff Butik Pasta", "Baterist Tasarım Pasta", "US Özel Pasta", "Karlar Ülkesi Tasarım Pasta", "Kalpli Ayıcık Tasarım Pasta", "Beşiktaş Butik Pasta" };

            String qba = ""; String qya = ""; String qbi = ""; String qyi = ""; String qtt = "";
            for (int i = 0; i < ba.Length; i++) { qba = qba + " N'" + ba[i] + "', "; }
            qba = qba.Substring(0, qba.Length - 2);
            for (int i = 0; i < ya.Length; i++) { qya = qya + " N'" + ya[i] + "', "; }
            qya = qya.Substring(0, qya.Length - 2);
            for (int i = 0; i < bi.Length; i++) { qbi = qbi + " N'" + bi[i] + "', "; }
            qbi = qbi.Substring(0, qbi.Length - 2);
            for (int i = 0; i < yi.Length; i++) { qyi = qyi + " N'" + yi[i] + "', "; }
            qyi = qyi.Substring(0, qyi.Length - 2);
            for (int i = 0; i < tt.Length; i++) { qtt = qtt + " N'" + tt[i] + "', "; }
            qtt = qtt.Substring(0, qtt.Length - 2);

            DataTable tasarim = dc.MemoryCacheByQuery("select * from MadeCakes where Name in (" + qtt + ")");

            DataTable butik_ank = dc.MemoryCacheByQuery("select * from Products where Name in  (" + qba + ")");
            DataTable butik_ist = dc.MemoryCacheByQuery("select * from Products where Name in (" + qbi + ")");

            DataTable yas_ank = dc.MemoryCacheByQuery("select * from Products where Name in (" + qya + ")");
            DataTable yas_ist = dc.MemoryCacheByQuery("select * from Products where Name in (" + qyi + ")");

            List<DataTable> tables = new List<DataTable>();
            tables.Add(tasarim); tables.Add(butik_ank); tables.Add(butik_ist); tables.Add(yas_ank); tables.Add(yas_ist);

            return tables;
        }

        public BlogFeed GetRSSFeed()
        {
            BlogFeed bf = (BlogFeed)MemoryCache.Default["BlogFeed"];

            if (bf == null)
            {
                bf = new BlogFeed(); bf.Posts = new List<SyndicationItem>();

                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("http://blog.icaked.com/feed");
                HttpWebResponse httpWebesponse = (HttpWebResponse)httpWebRequest.GetResponse();

                Stream dataStream = httpWebesponse.GetResponseStream();
                StreamReader streamreader = new StreamReader(dataStream, Encoding.UTF8);
                string response = streamreader.ReadToEnd();
                streamreader.Close();

                response = response.Replace((char)0x10, ' ').Replace("&#8230;", " ").Replace("/wp-content", "http://blog.icaked.com/wp-content");

                XmlTextReader reader = new XmlTextReader(new System.IO.StringReader(response));
                reader.Read();

                SyndicationFeed feed = SyndicationFeed.Load(reader);

                foreach (var post in feed.Items.Take(3))
                {
                    bf.Posts.Add(post);
                }

                CacheItemPolicy policy = new CacheItemPolicy();
                policy.AbsoluteExpiration = new DateTimeOffset(DateTime.Now.AddHours(1));
                MemoryCache.Default.Add("BlogFeed", bf, policy);
            }

            return bf;
        }

        public string categoryCorrector(string input)
        {
            string category = "";
            if (input == "dogum-gunu-pastalari") { category = "Doğum Günü Pastaları"; }
            else if (input == "bebek-pastalari") { category = "Bebek Pastaları"; }
            else if (input == "1-yas-pastalari") { category = "1 Yaş Pastaları"; }
            else if (input == "cizgi-film-pastalari") { category = "Çizgi Film Pastaları"; }
            else if (input == "sevgili-pastalari") { category = "Sevgili Pastaları"; }
            else if (input == "kutlama-pastalari") { category = "Kutlama Pastaları"; }
            else if (input == "butik-pasta") { category = "Butik Pasta"; }
            else if (input == "yas-pasta") { category = "Yaş Pasta"; }
            else if (input == "cikolata") { category = "Çikolata"; }
            else if (input == "tatli") { category = "Tatlı"; }
            return category;
        }

        public string UppercaseFirst(string s)
        {
            string[] elems = new string[0];
            if (s.Contains('-')) { elems = s.Split('-'); }
            else if (s.Contains(' ')) { elems = s.Split(' '); }
            else { elems = new string[1] { s }; }

            for (int i = 0; i < elems.Length; i++)
            {
                string str = elems[i];
                if (String.IsNullOrEmpty(str) == false)
                {
                    elems[i] = char.ToUpper(str[0]) + str.Substring(1);
                }
            }

            return String.Join(" ", elems);
        }

        public string generateReview()
        {
            Random rnd = new Random();
            int view = rnd.Next(10, 50);
            return view.ToString();
        }

        public string CalculateProductRating(string productID)
        {
            Random rnd = new Random();
            int rank = rnd.Next(40, 50);
            //int maxRating = Convert.ToInt32(dc.DBQueryGetter("select MAX(Rating) from Products").Rows[0][0]);
            //int proRating = Convert.ToInt32(dc.DBQueryGetter("select Rating from Products where ProductID = '" + new Guid(productID) + "'").Rows[0][0]);

            //double rating = proRating*5 / maxRating;
            return ((double)rank / 10).ToString().Replace(",", ".");
        }

        public string CalculateDesignRating(string madeID)
        {
            Random rnd = new Random();
            int rank = rnd.Next(40, 50);
            //int maxRating = Convert.ToInt32(dc.DBQueryGetter("select MAX(Likes) from MadeCakes").Rows[0][0]);
            //int proRating = Convert.ToInt32(dc.DBQueryGetter("select Likes from MadeCakes where MadeID = '" + new Guid(madeID) + "'").Rows[0][0]);

            //double rating = proRating * 5 / maxRating;
            //return rating.ToString();
            return ((double)rank / 10).ToString().Replace(",", ".");
        }

        public string SerializeDatatable(DataTable dt)
        {
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;
            foreach (DataRow dr in dt.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }
            return serializer.Serialize(rows);
        }

        public string cleanseUrlString(string urlStr)
        {
            if (urlStr == "cake") { urlStr = "pasta"; }
            else if (urlStr == "cupcake") { urlStr = "cupcake"; }   // lol
            else if (urlStr == "cookie") { urlStr = "kurabiye"; }
            urlStr = urlStr.ToLower().Replace("/", "-").Replace(" ", "-").Replace("ı", "i").Replace("ü", "u").Replace("ç", "c").Replace("ş", "s").Replace("ğ", "g").Replace("ö", "o");

            Regex rgx = new Regex("[^a-zA-Z0-9 -]");
            urlStr = rgx.Replace(urlStr, "");

            return urlStr;
        }

        public string ArrangePrice(string price)
        {
            price = price.Replace(",", ".");
            if (price.Contains("."))
            {
                if (price.Split('.')[1].Length == 1) { price = price + "0"; }
            }
            else { price = price + ".00"; }

            return price;
        }

        public string CalculateCartWithoutKDV(List<DataRow> cart)
        {
            Double price = 0;
            for (int i = 0; i < cart.Count; i++)
            {
                Double tempPrice = Convert.ToDouble(cart[i]["PagePrice"].ToString().Replace(".", ","));
                if (cart[i]["Category"].ToString() == "Parti Malzemeleri") { price = (tempPrice * 100) / 118; }
                else { price = price + tempPrice * 0.92; }
            }

            return string.Format("{0:0.00}", price);
        }

        public string CalculateCartKDV(List<DataRow> cart)
        {
            Double price = 0;
            for (int i = 0; i < cart.Count; i++)
            {
                Double tempPrice = Convert.ToDouble(cart[i]["PagePrice"].ToString().Replace(".", ","));
                if (cart[i]["Category"].ToString() == "Parti Malzemeleri") { price = (tempPrice * 18) / 118; }
                else { price = price + tempPrice * 0.08; }
            }

            return string.Format("{0:0.00}", price);
        }


        //public void gatherImages()
        //{
        //    int idx = 0;
        //    string[] directories = Directory.GetDirectories("C:\\Users\\Cagdas Umay\\Documents\\Visual Studio 2015\\Projects\\Cake\\Cake\\Images\\MadeCakes");
        //    for(int i = 0; i < directories.Length; i++)
        //    {
        //        string[] files = Directory.GetFiles(directories[i]);
        //        for(int i2 = 0; i2 < files.Length; i2++)
        //        {
        //            string fileToCopy = files[i2];
        //            string destinationDirectory = "C:\\Users\\Cagdas Umay\\Desktop\\AllImages";
        //            System.IO.File.Copy(fileToCopy, destinationDirectory + "\\" + idx.ToString() + ".png");
        //            idx++;
        //        }
        //    }
        //}
    }
}