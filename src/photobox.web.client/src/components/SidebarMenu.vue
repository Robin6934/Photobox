<template>
  <div class="layout">
    <!-- Sidebar -->
    <nav>
      <ul>
        <li><router-link to="/">Home</router-link></li>
        <li><router-link to="/gallery/067548">Gallery</router-link></li>
      </ul>
    </nav>
  </div>
</template>

<script setup lang="ts">
</script>

<style scoped>
.layout {
  display: flex;
  height: 100vh;
}

/* Sidebar styling */
.sidebar {
  width: 250px;
  background-color: #1e1e2f;
  color: white;
  transition: transform 0.3s ease;
  transform: translateX(-100%);
  overflow-y: auto;
  padding: 1rem;
}

.sidebar.open {
  transform: translateX(0);
}

.sidebar nav ul {
  list-style: none;
  padding: 0;
}

.sidebar nav ul li {
  margin: 1rem 0;
}

.sidebar nav ul li a {
  color: white;
  text-decoration: none;
}

.sidebar nav ul li a.router-link-active {
  font-weight: bold;
}

/* Main content */
.main {
  flex: 1;
  position: relative;
  padding: 1rem;
}

/* Burger button */
.burger {
  position: absolute;
  top: 1rem;
  left: 1rem;
  font-size: 1.5rem;
  background: none;
  border: none;
  cursor: pointer;
  color: #333;
}
</style>
