var courseLink = "";
const link1 = "https://lh5.googleusercontent.com/KSbz6mkRz75W3B3xVorFAtp5LpOBPZ3QHrhsPGmxkupzI21AYgZF6pUGj6Cl9Z-6HS0=w2400";
const link2 = "https://lh3.googleusercontent.com/SIzkmZNI8Vxthfy2GVRG2s49BVula1sAWEGmdQcjuyeRqoWth1mKMkXq3eZFwddgqPk=w2400";
const link3 = "https://lh5.googleusercontent.com/x3sjUZGu1Tc1Drh2QUDatIy-q-ZqtDwj-PFuI_I__BI3qjX4nSii3LM9agC1T9pHYwo=w2400";
const link4 = "https://lh4.googleusercontent.com/PLLtAWYWwKH3oosBzV_eILc2IECSMzMFwgJe6tSEg1crdVhiYpKNityQ3EMwL1lpvPQ=w2400";
const link5 = "https://lh4.googleusercontent.com/WQ_qnPF8hNJ2Tg5RvyjHN8V_nAKpSYAZBLuXsZtRG67pX_471uwUo-orAwk6RfPqSlM=w2400";

document.addEventListener("DOMContentLoaded", function(event) {
    var spc = document.getElementById("startingpointCourses");
    spc.addEventListener("change", (e) => onStartChoice(e.target.value));
});

function onStartChoice(value) {
    switch (value) {
        case '1':
            courseLink = link1;
            break;
        case '2':
            courseLink = link2;
            break;
        case '3':
            courseLink = link3;
            break;
        case '4':
            courseLink = link4;
            break;
        case '5':
            courseLink = link5;
            break;
    }

    const imgContainer = document.getElementById("imageContainer").src=courseLink;

}