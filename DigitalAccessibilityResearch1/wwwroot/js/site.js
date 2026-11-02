
document.addEventListener("DOMContentLoaded", function () {
    const body = document.body;
    const root = document.documentElement;

    // Accessibility toolbar buttons
    const decreaseButton = document.getElementById("decreaseText");
    const increaseButton = document.getElementById("increaseText");
    const contrastButton = document.getElementById("contrastToggle");
    const darkModeButton = document.getElementById("darkModeToggle");
    const readingModeButton = document.getElementById("readingModeToggle");
    const spacingButton = document.getElementById("lineSpacingToggle");
    const resetButton = document.getElementById("resetDisplay");

    const STORAGE_KEY = "accessLearnAccessibility";

    const defaultSettings = {
        textScale: 100,
        highContrast: false,
        darkMode: false,
        readingMode: false,
        lineSpacing: false
    };

    let settings = { ...defaultSettings };

    // Load saved accessibility settings
    function loadSettings() {
        try {
            const savedSettings = localStorage.getItem(STORAGE_KEY);

            if (savedSettings) {
                settings = {
                    ...defaultSettings,
                    ...JSON.parse(savedSettings)
                };
            }
        } catch (error) {
            console.error("Unable to load accessibility settings:", error);
        }

        settings.textScale = Math.max(
            80,
            Math.min(150, Number(settings.textScale) || 100)
        );
    }

    // Save settings to localStorage
    function saveSettings() {
        try {
            localStorage.setItem(
                STORAGE_KEY,
                JSON.stringify(settings)
            );
        } catch (error) {
            console.error("Unable to save accessibility settings:", error);
        }
    }

    // Update button accessibility states
    function updateButtonStates() {
        if (contrastButton) {
            contrastButton.setAttribute(
                "aria-pressed",
                String(settings.highContrast)
            );
        }

        if (darkModeButton) {
            darkModeButton.setAttribute(
                "aria-pressed",
                String(settings.darkMode)
            );
        }

        if (readingModeButton) {
            readingModeButton.setAttribute(
                "aria-pressed",
                String(settings.readingMode)
            );
        }

        if (spacingButton) {
            spacingButton.setAttribute(
                "aria-pressed",
                String(settings.lineSpacing)
            );
        }
    }

    // Apply all accessibility settings
    function applySettings() {
        root.style.setProperty(
            "--text-scale",
            settings.textScale + "%"
        );

        body.classList.toggle(
            "high-contrast",
            settings.highContrast
        );

        body.classList.toggle(
            "dark-mode",
            settings.darkMode
        );

        body.classList.toggle(
            "reading-mode",
            settings.readingMode
        );

        body.classList.toggle(
            "large-line-spacing",
            settings.lineSpacing
        );

        updateButtonStates();
        saveSettings();
    }

    // Decrease text size
    if (decreaseButton) {
        decreaseButton.addEventListener("click", function () {
            settings.textScale = Math.max(
                80,
                settings.textScale - 10
            );

            applySettings();
        });
    }

    // Increase text size
    if (increaseButton) {
        increaseButton.addEventListener("click", function () {
            settings.textScale = Math.min(
                150,
                settings.textScale + 10
            );

            applySettings();
        });
    }

    // Toggle high contrast
    if (contrastButton) {
        contrastButton.addEventListener("click", function () {
            settings.highContrast = !settings.highContrast;
            applySettings();
        });
    }

    // Toggle dark mode
    if (darkModeButton) {
        darkModeButton.addEventListener("click", function () {
            settings.darkMode = !settings.darkMode;
            applySettings();
        });
    }

    // Toggle reading mode
    if (readingModeButton) {
        readingModeButton.addEventListener("click", function () {
            settings.readingMode = !settings.readingMode;
            applySettings();
        });
    }

    // Toggle increased line spacing
    if (spacingButton) {
        spacingButton.addEventListener("click", function () {
            settings.lineSpacing = !settings.lineSpacing;
            applySettings();
        });
    }

    // Reset all accessibility settings
    if (resetButton) {
        resetButton.addEventListener("click", function () {
            settings = { ...defaultSettings };
            applySettings();
        });
    }

    // Initialize accessibility settings
    loadSettings();
    applySettings();
});


// ========================================
// TEXT-TO-SPEECH
// ========================================

document.addEventListener("DOMContentLoaded", function () {
    const status = document.getElementById("speech-status");

    // Check browser support
    if (!("speechSynthesis" in window)) {
        if (status) {
            status.textContent =
                "Text-to-Speech is not supported by this browser.";
        }

        document.querySelectorAll(".speech-button").forEach(button => {
            button.disabled = true;
        });

        return;
    }

    const speech = window.speechSynthesis;

    let currentCard = null;
    let currentUtterance = null;

    function getCardText(card) {
        const category = card.querySelector(".lesson-category")?.innerText || "";
        const title = card.querySelector(".lesson-title")?.innerText || "";
        const description =
            card.querySelector(".lesson-description")?.innerText || "";
        const duration =
            card.querySelector(".lesson-duration")?.innerText || "";

        return `${category}. ${title}. ${description}. ${duration}`;
    }

    function updateControls(card, speaking) {
        if (!card) return;

        const pauseButton =
            card.querySelector('[data-speech-action="pause"]');

        const stopButton =
            card.querySelector('[data-speech-action="stop"]');

        if (pauseButton) {
            pauseButton.disabled = !speaking;
            pauseButton.textContent = "Pause";
        }

        if (stopButton) {
            stopButton.disabled = !speaking;
        }
    }

    function stopSpeech() {
        speech.cancel();

        if (currentCard) {
            updateControls(currentCard, false);
        }

        currentCard = null;
        currentUtterance = null;
    }

    document.querySelectorAll(".speech-button").forEach(button => {
        button.addEventListener("click", function () {
            const action = this.dataset.speechAction;
            const card = this.closest(".lesson-card");

            if (!card) return;

            // Listen
            if (action === "listen") {
                stopSpeech();

                const text = getCardText(card);

                currentCard = card;
                currentUtterance = new SpeechSynthesisUtterance(text);

                currentUtterance.lang = "en-US";
                currentUtterance.rate = 1;
                currentUtterance.pitch = 1;

                currentUtterance.onstart = function () {
                    updateControls(card, true);

                    if (status) {
                        status.textContent = "Reading lesson content.";
                    }
                };

                currentUtterance.onend = function () {
                    updateControls(card, false);

                    if (status) {
                        status.textContent = "Finished reading lesson.";
                    }

                    currentCard = null;
                    currentUtterance = null;
                };

                currentUtterance.onerror = function () {
                    updateControls(card, false);

                    if (status) {
                        status.textContent =
                            "Unable to read this lesson.";
                    }

                    currentCard = null;
                    currentUtterance = null;
                };

                speech.speak(currentUtterance);
            }

            // Pause or resume
            if (action === "pause" && currentCard === card) {
                if (speech.paused) {
                    speech.resume();
                    this.textContent = "Pause";

                    if (status) {
                        status.textContent = "Reading resumed.";
                    }
                } else if (speech.speaking) {
                    speech.pause();
                    this.textContent = "Resume";

                    if (status) {
                        status.textContent = "Reading paused.";
                    }
                }
            }

            // Stop
            if (action === "stop" && currentCard === card) {
                stopSpeech();

                if (status) {
                    status.textContent = "Reading stopped.";
                }
            }
        });
    });

    // Stop speech when leaving the page
    window.addEventListener("pagehide", stopSpeech);
});


// ========================================
// TEXT-TO-SPEECH - LESSON DETAILS
// ========================================

document.addEventListener("DOMContentLoaded", function () {
    const content = document.getElementById("lesson-speech-content");
    const playButton = document.getElementById("lesson-speech-play");
    const pauseButton = document.getElementById("lesson-speech-pause");
    const stopButton = document.getElementById("lesson-speech-stop");
    const status = document.getElementById("lesson-speech-status");

    // Only run on the lesson details page
    if (!content || !playButton || !pauseButton || !stopButton) {
        return;
    }

    if (!("speechSynthesis" in window)) {
        status.textContent =
            "Text-to-Speech is not supported by this browser.";

        playButton.disabled = true;
        return;
    }

    const speech = window.speechSynthesis;
    let utterance = null;

    function updateButtons(isReading) {
        playButton.disabled = isReading;
        pauseButton.disabled = !isReading;
        stopButton.disabled = !isReading;
        pauseButton.textContent = "Pause";
    }

    function stopReading() {
        speech.cancel();
        utterance = null;
        updateButtons(false);
    }

    playButton.addEventListener("click", function () {
        stopReading();

        const text = content.innerText.trim();

        if (!text) {
            status.textContent = "There is no lesson content to read.";
            return;
        }

        utterance = new SpeechSynthesisUtterance(text);

        utterance.lang = "en-US";
        utterance.rate = 1;
        utterance.pitch = 1;

        utterance.onstart = function () {
            updateButtons(true);
            status.textContent = "Reading lesson content.";
        };

        utterance.onend = function () {
            updateButtons(false);
            status.textContent = "Finished reading the lesson.";
            utterance = null;
        };

        utterance.onerror = function () {
            updateButtons(false);
            status.textContent = "Unable to read the lesson.";
            utterance = null;
        };

        speech.speak(utterance);
    });

    pauseButton.addEventListener("click", function () {
        if (speech.speaking && !speech.paused) {
            speech.pause();
            pauseButton.textContent = "Resume";
            status.textContent = "Reading paused.";
        } else if (speech.paused) {
            speech.resume();
            pauseButton.textContent = "Pause";
            status.textContent = "Reading resumed.";
        }
    });

    stopButton.addEventListener("click", function () {
        stopReading();
        status.textContent = "Reading stopped.";
    });

    window.addEventListener("pagehide", stopReading);
});

