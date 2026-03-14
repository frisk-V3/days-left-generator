document.addEventListener('DOMContentLoaded', () => {
    const dateInput = document.getElementById('target-date');
    const resultContainer = document.getElementById('result-container');
    const daysCount = document.getElementById('days-count');

    // 日付が変更された時の処理
    dateInput.addEventListener('change', () => {
        const targetDate = new Date(dateInput.value);
        const today = new Date();

        // 時間を00:00:00にリセットして純粋な「日数」を計算
        targetDate.setHours(0, 0, 0, 0);
        today.setHours(0, 0, 0, 0);

        // 差分をミリ秒で取得
        const diffTime = targetDate - today;
        
        // ミリ秒を日にちに変換 (1日 = 24時間 * 60分 * 60秒 * 1000ミリ秒)
        const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));

        if (!isNaN(diffDays)) {
            resultContainer.classList.remove('hidden');
            daysCount.textContent = diffDays;
            
            // 過去の日付の場合は色を変えるなどの調整も可能
            if (diffDays < 0) {
                daysCount.style.color = "#ff4d4f"; // 過ぎている場合は赤
            } else {
                daysCount.style.color = "#007bff"; // 未来の場合は青
            }
        }
    });
});
