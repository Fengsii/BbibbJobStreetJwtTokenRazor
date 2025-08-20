
window.addEventListener('scroll', function() {
  const footer = document.getElementById('footer');
  const elements = footer.querySelectorAll('.footer-content, .footer-bottom, .footer-column, .social-icon, .footer-link, .newsletter-form');
  const footerTop = footer.getBoundingClientRect().top;
  const windowHeight = window.innerHeight;

  if (footerTop < windowHeight * 0.9) {
    elements.forEach(element => {
      element.classList.add('visible');
    });
  }
});

// Check on initial load
window.addEventListener('load', function() {
  const footer = document.getElementById('footer');
  const elements = footer.querySelectorAll('.footer-content, .footer-bottom, .footer-column, .social-icon, .footer-link, .newsletter-form');
  const footerTop = footer.getBoundingClientRect().top;
  const windowHeight = window.innerHeight;

  if (footerTop < windowHeight * 0.9) {
    elements.forEach(element => {
      element.classList.add('visible');
    });
  }
});
