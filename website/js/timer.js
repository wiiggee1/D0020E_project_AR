document.addEventListener('DOMContentLoaded', () => {
const startBtn = document.getElementById('startbtn');
const timerElement = document.getElementById('timer');
const maxTime = 18000; // 3 minutes in deciseconds
let timeRemaining = maxTime;
let intervalID;

if (startBtn) {
    startBtn.addEventListener('click', () => {
    intervalID = setInterval(() => {
    timeRemaining -= 10;

    const minutes = Math.floor(timeRemaining / 6000);
    const seconds = Math.floor((timeRemaining % 6000) / 100);
    const deciseconds = timeRemaining % 100;

    const timeString = `${String(minutes).padStart(2, '0')}:${String(seconds).padStart(2, '0')}:${String(deciseconds).padStart(2, '0')}`;
    timerElement.innerText = timeString;

    if (timeRemaining <= 0) {
      clearInterval(intervalID);
      }
    }, 100);
  });
}
});