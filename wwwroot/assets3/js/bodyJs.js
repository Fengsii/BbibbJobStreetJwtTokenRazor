
    const products = [
      {
        id: 1,
        title: "Kaos Oversize Premium",
        brand: "Brand Fashion",
        currentPrice: "Rp 199.000",
        originalPrice: "Rp 299.000",
        rating: 4.5,
        reviewCount: 128,
        image: "/api/placeholder/400/320",
        isBestseller: true
      },
      {
        id: 2,
        title: "Kemeja Linen Premium",
        brand: "Brand Fashion",
        currentPrice: "Rp 329.000",
        originalPrice: "Rp 459.000",
        rating: 4.0,
        reviewCount: 87,
        image: "/api/placeholder/400/320",
        isBestseller: false
      },
      {
        id: 3,
        title: "Celana Jogger Katun",
        brand: "Brand Fashion",
        currentPrice: "Rp 249.000",
        originalPrice: "Rp 349.000",
        rating: 5.0,
        reviewCount: 215,
        image: "/api/placeholder/400/320",
        isBestseller: true
      },
      {
        id: 4,
        title: "Tas Kanvas Minimalis",
        brand: "Brand Fashion",
        currentPrice: "Rp 179.000",
        originalPrice: "Rp 259.000",
        rating: 3.5,
        reviewCount: 76,
        image: "/api/placeholder/400/320",
        isBestseller: false
      },
      {
        id: 5,
        title: "Jaket Denim Klasik",
        brand: "Brand Fashion",
        currentPrice: "Rp 459.000",
        originalPrice: "Rp 599.000",
        rating: 4.5,
        reviewCount: 142,
        image: "/api/placeholder/400/320",
        isBestseller: false
      },
      {
        id: 6,
        title: "Dress Casual Elegan",
        brand: "Brand Fashion",
        currentPrice: "Rp 359.000",
        originalPrice: "Rp 499.000",
        rating: 5.0,
        reviewCount: 189,
        image: "/api/placeholder/400/320",
        isBestseller: true
      },
      {
        id: 7,
        title: "Topi Baseball Vintage",
        brand: "Brand Fashion",
        currentPrice: "Rp 129.000",
        originalPrice: "Rp 179.000",
        rating: 4.0,
        reviewCount: 93,
        image: "/api/placeholder/400/320",
        isBestseller: false
      },
      {
        id: 8,
        title: "Sepatu Sneakers Urban",
        brand: "Brand Fashion",
        currentPrice: "Rp 499.000",
        originalPrice: "Rp 699.000",
        rating: 4.5,
        reviewCount: 247,
        image: "/api/placeholder/400/320",
        isBestseller: true
      }
    ];

    // Function to render star ratings
    function renderStars(rating) {
      let starsHTML = '';
      const fullStars = Math.floor(rating);
      const hasHalfStar = rating % 1 !== 0;
      
      for (let i = 0; i < fullStars; i++) {
        starsHTML += '<i class="fas fa-star"></i>';
      }
      
      if (hasHalfStar) {
        starsHTML += '<i class="fas fa-star-half-alt"></i>';
      }
      
      const emptyStars = 5 - fullStars - (hasHalfStar ? 1 : 0);
      for (let i = 0; i < emptyStars; i++) {
        starsHTML += '<i class="far fa-star"></i>';
      }
      
      return starsHTML;
    }

    // Function to render products
    function renderProducts() {
      const productGrid = document.getElementById('productGrid');
      productGrid.innerHTML = '';
      
      products.forEach((product, index) => {
        const productCard = document.createElement('div');
        productCard.className = 'product-card';
        productCard.dataset.index = index;
        
        const productHTML = `
          <div class="product-image">
            <img src="${product.image}" alt="${product.title}">
            ${product.isBestseller ? '<div class="bestseller-badge">Terlaris</div>' : ''}
            <div class="quick-view">Lihat Cepat</div>
          </div>
          <div class="product-info">
            <h3 class="product-title">${product.title}</h3>
            <p class="product-brand">${product.brand}</p>
            <div class="product-price">
              <span class="current-price">${product.currentPrice}</span>
              <span class="original-price">${product.originalPrice}</span>
            </div>
            <div class="product-rating">
              <div class="stars">
                ${renderStars(product.rating)}
              </div>
              <span class="review-count">(${product.reviewCount})</span>
            </div>
            <button class="add-to-cart">
              <i class="fas fa-shopping-cart"></i>
              Tambahkan ke Keranjang
            </button>
          </div>
        `;
        
        productCard.innerHTML = productHTML;
        productGrid.appendChild(productCard);
      });
    }

    // Initialize products
    renderProducts();
    
    // User Profile Dropdown Toggle
    const profileIcon = document.getElementById('profileIcon');
    const profileDropdown = document.getElementById('profileDropdown');
    
    profileIcon.addEventListener('click', function() {
      profileDropdown.classList.toggle('active');
    });
    
    // Close dropdown when clicking outside
    document.addEventListener('click', function(event) {
      if (!profileIcon.contains(event.target) && !profileDropdown.contains(event.target)) {
        profileDropdown.classList.remove('active');
      }
    });
    
    // Scroll animation for products
    function checkProductVisibility() {
      const productCards = document.querySelectorAll('.product-card');
      const windowHeight = window.innerHeight;
      
      productCards.forEach((card, index) => {
        const cardTop = card.getBoundingClientRect().top;
        
        if (cardTop < windowHeight - 100) {
          setTimeout(() => {
            card.classList.add('animated');
          }, index * 100);
        }
      });
    }
    
    // Check on initial load
    window.addEventListener('load', checkProductVisibility);
    
    // Check on scroll
    window.addEventListener('scroll', checkProductVisibility);
