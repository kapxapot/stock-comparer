const uri = 'api/Performance/';
var chart = null;

function showError(error) {
    const errorEl = document.getElementById('error');

    errorEl.style.display = 'block';
    errorEl.innerText = error;
}

function hideError() {
    const errorEl = document.getElementById('error');

    errorEl.style.display = 'none';
    errorEl.innerText = '';
}

function convertData(symbol, data) {
    return {
        labels: data.map(d => moment(d.date).format('MMM D')),
        datasets: [
            {
                label: symbol,
                data: data.map(d => Number.parseFloat(d.delta).toFixed(4)),
                backgroundColor: pickColor(0, 0, 0.2),
                borderColor: pickColor(0, 0),
                borderWidth: 1
            },
            {
                label: 'SPY',
                data: data.map(d => Number.parseFloat(d.spyDelta).toFixed(4)),
                backgroundColor: pickColor(1, 0, 0.2),
                borderColor: pickColor(1, 0),
                borderWidth: 1
            }
        ]
    };
}

function bindData(symbol, data) {
    const chartData = convertData(symbol, data);

    const ctx = document.getElementById('chart-canvas').getContext('2d');

    if (!chart) {
        chart = new Chart(ctx, {
            type: 'bar',
            data: chartData,
            options: {
                responsive: true
            }
        });
    } else {
        chart.data = chartData;
        chart.update();
    }
}

function showChart(symbol, data) {
    const chartEl = document.getElementById('chart');

    bindData(symbol, data);

    chartEl.style.display = 'block';
}

function hideChart() {
    const chartEl = document.getElementById('chart');

    chartEl.style.display = 'none';
}

function loadingOn() {
    document.getElementById('the-button').disabled = true;
    document.getElementById('loading').style.display = 'block';
}

function loadingOff() {
    document.getElementById('the-button').disabled = false;
    document.getElementById('loading').style.display = 'none';
}

function compare() {
    hideChart();

    let symbol = document.getElementById('symbol').value;
    
    if (symbol) {
        symbol = symbol.trim();
    }

    if (!symbol) {
        showError('Please, enter the stock symbol.');
        return;
    }

    hideError();
    loadingOn();

    fetch(uri + symbol)
        .then(response => {
            if (response.ok) {
                return response.json();
            }

            let error = {
                status: response.status,
                statusText: response.statusText
            };

            const contentType = response.headers.get("content-type");

            if (contentType
                && (contentType.indexOf("application/json") !== -1
                    || contentType.indexOf("application/problem+json") !== -1)
            ) {
                error.json = response.json();
            } else {
                error.text = response.text();
            }

            return Promise.reject(error);
        })
        .then(data => {
            showChart(symbol, data);
            loadingOff();
        })
        .catch(error => {
            if (error.json) {
                error.json.then(data => {
                    showError(data.detail);
                });
            } else {
                showError('Failed to load data: ' + error.status + ' ' + error.statusText);
            }

            loadingOff();
        });
}
