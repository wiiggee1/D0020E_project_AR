document.addEventListener('DOMContentLoaded', () => {
  const startBtn = document.getElementById('startbtn');
  const timerElement = document.getElementById('timer');
  const maxTime = 180000; // 3 minutes in miliseconds 
  let timeRemaining = maxTime;
  let intervalID;
  const hit_timestamps = [60000, 120000]; // 1 minute and 2 minutes in milliseconds

  const startTimer = (maxTime) => { 
    clearInterval(intervalID);
    timeRemaining = maxTime;

    intervalID = setInterval(() => {
      if (hit_timestamps.includes(timeRemaining)) {
        timeRemaining -= 10000; // subtract 10 seconds
      } else {
        timeRemaining -= 10; // subtract 10 centiseconds
      }

      const minutes = Math.floor(timeRemaining / 60000);
      const seconds = Math.floor((timeRemaining % 60000) / 1000);
      const centiseconds = Math.floor((timeRemaining % 1000) / 10);

      const timeString = `${String(minutes).padStart(2, '0')}:${String(seconds).padStart(2, '0')}:${String(centiseconds).padStart(2, '0')}`;
      timerElement.innerText = timeString;

      if (timeRemaining <= 0) {
        clearInterval(intervalID);
      }
    }, 10);
  };

  startBtn.addEventListener('click', () => {
    startTimer(maxTime);
  });
});
