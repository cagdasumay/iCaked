# Cake3D

Kurulum ve geliştirme dokümantasyonudur.

## Build Requirements

* NodeJS: İndirmek için [NodeJS](https://nodejs.org/en/download/)
* npm: NodeJS ile geliyor.
* [Webpack](https://webpack.github.io/)
* [THREE.JS](https://threejs.org)

## Kurulum

### NodeJS

NodeJS kurulumu [şurada](http://blog.teamtreehouse.com/install-node-js-npm-windows) anlatılıyor. NodeJS paket yöneticisi ```npm``` beraberinde kuruluyor.

### Webpack

Dev bir Javascript dosyasını düzenlemektense, projeyi moduller halinde geliştirip, biribirlerini import edip, en son paket haline getirmek daha avantajlı. Webpack bu şekilde modul halindeki .js dosyalarını tek bir derlenmiş, hatta sıkıştırılmış dosya haline getirip HTML &lt;script&gt; tagı ile sayfaya koyabilmeyi sağlıyor. Yüklenişi:

```
npm install webpack -g
```

Yukarıdaki komut ```webpack```i global olarak yükler. Windows'ta PATH'e eklemek şeklinde birkaç ayarlama gerekebilir.

### Github repo

```
git clone https://github.com/InfiniaEngineering/Cake3D.git
```

### THREE.JS

Cake3D klasörü içine gelip (klasörde ```package.json``` bulunmalı).

```
npm install
```

Bu komutla THREE.JS ve THREEJS OBJLoader yüklenmiş olur. Aşağıdaki ile derlenip tek bir ```pasta.js``` haline gelir.


```
webpack
```

Browser'da ```page.html``` açılır.
